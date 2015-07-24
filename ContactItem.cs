using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailClient
{
    public class ContactItem
    {
        public string Name{set;get;}
        public string Email { set; get; }
        public string Note { set; get; }
        //public bool Selected { set; get; }

        public static string StoreContacst(List<ContactItem> list)
        {
            string result = string.Empty;
            foreach (ContactItem item in list)
            {
                result += string.Format("{0};{1};{2};", item.Name, item.Email, item.Note);
            }
            return result;
        }

        public static List<ContactItem> LoadContacst(string contacts)
        {
            List<ContactItem> result = new List<ContactItem>();
            string[] address = contacts.Split(';');

            for (int i = 0; i < address.Length - 1; i += 3)
            {
                result.Add(new ContactItem() { Name = address[i].Trim(), Email = address[i + 1].Trim(), Note = address[i + 2].Trim(), /*Selected = false*/ });
            }
            return result;
        }
    }
}
