using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace KnowledgeSystem.Views._03_DepartmentManage._18_SharedTaskManagement
{
    public sealed class Task318Attachment
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string SizeText { get; set; }
        public string SourcePath { get; set; }
    }

    public sealed class Task318Draft
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Assignee { get; set; }
        public string SharedRootPath { get; set; }
        public string TargetFolderPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public Image SourceImage { get; set; }
        public BindingList<Task318Attachment> Attachments { get; } =
            new BindingList<Task318Attachment>();

        public int AttachmentCount => Attachments.Count;

        public void ReplaceAttachments(IEnumerable<Task318Attachment> attachments)
        {
            Attachments.Clear();
            if (attachments == null) return;

            foreach (var attachment in attachments)
                Attachments.Add(attachment);
        }
    }
}
