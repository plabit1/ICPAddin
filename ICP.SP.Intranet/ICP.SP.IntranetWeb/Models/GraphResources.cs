using System;
using System.Collections.Generic;

namespace ICP.SP.IntranetWeb.Models
{
    public class UserInfo
    {
        public string Address { get; set; }
    }

    public class Message
    {
        public string Subject { get; set; }
        public ItemBody Body { get; set; }
        public List<Recipient> ToRecipients { get; set; }
    }

    public class Recipient
    {
        public UserInfo EmailAddress { get; set; }
    }

    public class ItemBody
    {
        public string ContentType { get; set; }
        public string Content { get; set; }
    }

    public class MessageRequest
    {
        public Message Message { get; set; }
        public bool SaveToSentItems { get; set; }
    }

    public class TaskInfo
    {
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public int PercentComplete { get; set; }
        public string Indicator { get; set; }
    }

    public class EventInfo
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsAllDay { get; set; }
        public string Location { get; set; }
    }
}