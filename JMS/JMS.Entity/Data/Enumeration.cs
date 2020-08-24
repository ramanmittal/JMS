using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Entity.Data
{
    public enum SubmissionStatus
    {
        Draft,
        Submission,
        Review,
        Rejected
    }
    public enum ContributerRole
    {
        Author,
        Translator
    }
    public enum ReviewType
    {
        DoubleBlind,
        Blind,
        Open
    }
    public enum ReviewerStatus
    {
        DoubleBlind,
        Blind,
        Open
    }
    public enum ReviewerAction { 
    
    }
}
