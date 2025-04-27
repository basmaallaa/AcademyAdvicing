using MimeKit;

namespace Academy.Core.Models.Email
{
    public class Message
    {
        // قائمة المستلمين (To)
        public List<MailboxAddress> To { get; set; }

        // عنوان البريد الإلكتروني (Subject)
        public string Subject { get; set; }

        // محتوى البريد الإلكتروني (Content)
        public string Content { get; set; }

        // Constructor
        public Message(IEnumerable<string> to, string subject, string content)
        {
            // إنشاء قائمة جديدة من العناوين البريدية
            To = new List<MailboxAddress>();

            // تحويل عناوين البريد من string إلى MailboxAddress
            To.AddRange(to.Select(x => new MailboxAddress("email", x)));

            // تعيين باقي القيم
            Subject = subject;
            Content = content;
        }
    }
}
