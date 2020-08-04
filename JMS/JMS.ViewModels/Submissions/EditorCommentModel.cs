using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.ViewModels.Submissions
{
    public class EditorCommentModel
    {
        public long Id { get; set; }
        [DisplayName("Comment for editor")]
        public string Comment { get; set; }
    }
}
