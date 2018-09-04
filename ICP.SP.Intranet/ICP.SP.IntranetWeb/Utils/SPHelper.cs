using ICP.SP.Intranet.Models.Common;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ICP.SP.IntranetWeb.Utils
{
    public class SPHelper
    {
        log4net.ILog log = log4net.LogManager.GetLogger("Test");
        public string CreateSPFolder(SharePointContext spContext, string docLibrary, string code)
        {
            var result = string.Empty;
            using (var clientContext = spContext.CreateAppOnlyClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    try
                    {
                        var relativeSite = clientContext.Url.Substring(clientContext.Url.IndexOf('/', 10));
                        var folder = clientContext.Web.GetFolderByServerRelativeUrl(relativeSite + docLibrary);
                        clientContext.Load(folder);
                        clientContext.ExecuteQuery();
                        folder = folder.Folders.Add(code.Replace('/', '-'));
                        clientContext.Load(folder);
                        clientContext.ExecuteQuery();
                        result = folder.ServerRelativeUrl;
                    }
                    catch
                    {

                    }
                }
            }
            return result;
        }

        public string UploadSPFile(SharePointContext spContext, HttpPostedFileBase tempFile, string documentFolder, string siteUrl, string prefix)
        {
            log.Info("- OutboxDocs - UploadSPFile");
            var result = string.Empty;
            using (var clientContext = spContext.CreateAppOnlyClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    try
                    {
                        var relativeSite = clientContext.Url.Substring(clientContext.Url.IndexOf('/', 10));
                        var folder = clientContext.Web.GetFolderByServerRelativeUrl(documentFolder);
                        clientContext.Load(folder);
                        clientContext.ExecuteQuery();
                        FileCreationInformation fci = new FileCreationInformation();
                        if (tempFile.ContentLength < 1000000) // ~1MB
                        {
                            using (System.IO.Stream inputStream = tempFile.InputStream)
                            {
                                var memoryStream = inputStream as System.IO.MemoryStream;
                                if (memoryStream == null)
                                {
                                    memoryStream = new System.IO.MemoryStream();
                                    inputStream.CopyTo(memoryStream);
                                }
                                fci.Content = memoryStream.ToArray();
                            }
                            fci.Url = documentFolder + "/" + prefix + tempFile.FileName;
                            fci.Overwrite = true;
                            Microsoft.SharePoint.Client.File fileToUpload = folder.Files.Add(fci);
                            clientContext.Load(fileToUpload);
                            clientContext.ExecuteQuery();
                            Uri hostUrl = new Uri(siteUrl);
                            result = "https://" + hostUrl.Host + fileToUpload.ServerRelativeUrl;
                        }
                        else if (tempFile.ContentLength < 5000000) // ~5MB
                        {
                            fci.ContentStream = tempFile.InputStream;
                            fci.Url = documentFolder + "/" + prefix + tempFile.FileName;
                            fci.Overwrite = true;
                            Microsoft.SharePoint.Client.File fileToUpload = folder.Files.Add(fci);
                            clientContext.Load(fileToUpload);
                            clientContext.ExecuteQuery();
                            Uri hostUrl = new Uri(siteUrl);
                            result = "https://" + hostUrl.Host + fileToUpload.ServerRelativeUrl;
                        }
                        if(tempFile.ContentLength >= 5000000) // > ~5MB
                        {
                            Guid uploadId = Guid.NewGuid();
                            Microsoft.SharePoint.Client.File uploadFile;
                            int blockSize = 4 * 1024 * 1024; // 5MB blocks
                            ClientResult<long> bytesUploaded = null;
                            byte[] buffer = new byte[blockSize];
                            Byte[] lastBuffer = null;
                            long fileoffset = 0;
                            long totalBytesRead = 0;
                            int bytesRead;
                            bool first = true;
                            bool last = false;
                            using (BinaryReader br = new BinaryReader(tempFile.InputStream))
                            {
                                while ((bytesRead = br.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    totalBytesRead = totalBytesRead + bytesRead;

                                    // You've reached the end of the file.
                                    if (totalBytesRead == tempFile.ContentLength)
                                    {
                                        last = true;
                                        // Copy to a new buffer that has the correct size.
                                        lastBuffer = new byte[bytesRead];
                                        Array.Copy(buffer, 0, lastBuffer, 0, bytesRead);
                                    }

                                    if (first)
                                    {
                                        using (MemoryStream contentStream = new MemoryStream())
                                        {
                                            // Add an empty file.
                                            FileCreationInformation fileInfo = new FileCreationInformation();
                                            fileInfo.ContentStream = contentStream;
                                            fileInfo.Url = prefix + tempFile.FileName;
                                            fileInfo.Overwrite = true;
                                            uploadFile = folder.Files.Add(fileInfo);

                                            // Start upload by uploading the first slice. 
                                            using (MemoryStream s = new MemoryStream(buffer))
                                            {
                                                // Call the start upload method on the first slice.
                                                bytesUploaded = uploadFile.StartUpload(uploadId, s);
                                                clientContext.ExecuteQuery();
                                                // fileoffset is the pointer where the next slice will be added.
                                                fileoffset = bytesUploaded.Value;
                                            }

                                            // You can only start the upload once.
                                            first = false;
                                        }
                                    }
                                    else
                                    {
                                        // Get a reference to your file.
                                        uploadFile = clientContext.Web.GetFileByServerRelativeUrl(folder.ServerRelativeUrl + System.IO.Path.AltDirectorySeparatorChar + prefix + tempFile.FileName);

                                        if (last)
                                        {
                                            // Is this the last slice of data?
                                            using (MemoryStream s = new MemoryStream(lastBuffer))
                                            {
                                                // End sliced upload by calling FinishUpload.
                                                uploadFile = uploadFile.FinishUpload(uploadId, fileoffset, s);
                                                clientContext.Load(uploadFile);
                                                clientContext.ExecuteQuery();

                                                // Return the file object for the uploaded file.
                                                Uri hostUrl = new Uri(siteUrl);
                                                result = "https://" + hostUrl.Host + uploadFile.ServerRelativeUrl;
                                            }
                                        }
                                        else
                                        {
                                            using (MemoryStream s = new MemoryStream(buffer))
                                            {
                                                // Continue sliced upload.
                                                bytesUploaded = uploadFile.ContinueUpload(uploadId, fileoffset, s);
                                                clientContext.ExecuteQuery();
                                                // Update fileoffset for the next slice.
                                                fileoffset = bytesUploaded.Value;
                                            }
                                        }
                                    }
                                } // while ((bytesRead = br.Read(buffer, 0, buffer.Length)) > 0)
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.ToString());
                    }
                }
            }
            return result;
        }
    }
}