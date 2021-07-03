using System;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;

namespace Ogma3.Data.InviteCodes
{
    public class InviteCode : BaseModel
    {
        private readonly string _code;
        public string Code
        {
            get => _code;
            init
            {
                NormalizedCode = value.ToUpper();
                _code = value;
            }
        }
        public string NormalizedCode { get; private set; }
        
        public OgmaUser? UsedBy { get; set; }
        public long? UsedById { get; set; }
        public OgmaUser IssuedBy { get; set; }
        public long IssuedById { get; set; }
        public DateTime? UsedDate { get; set; }
        public DateTime IssueDate { get; set; }
    }
}