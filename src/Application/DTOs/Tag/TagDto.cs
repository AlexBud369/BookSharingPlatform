using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tag;

public class TagDto
{
    public Guid Id { get; set; }
    public string TagName { get; set; }
    public IEnumerable<Guid> BookIds { get; set; }
}
