using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Entities
{
    /// <summary>
    /// The entity of a tag is used to categorize books (genre, language)
    /// </summary>
    public class Tag
    {
        private Guid tagId;
        private string tagName;
        private List<Book> books;

        private Tag() 
        {
            books = new List<Book>();
        }

        public Tag(Guid tagId, string tagName)
        {
            tagId = Guid.NewGuid();
            this.tagName = tagName;
            books = new List<Book>();
        }

        public Guid TagId
        {
            get { return tagId; }
        }

        public string TagName {  
            get { return tagName; }
            private set { tagName = value; }
        }
        public void UpdateTagName(string newTagName)
        {
            TagName = newTagName;
        }

        public List<Book> Books
        {
            get { return books; }
        }
    }
}
