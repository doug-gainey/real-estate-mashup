using System.Collections.Generic;

namespace RealEstateMashup.Models
{
    public class ForeclosuresModel
    {
        public ForeclosuresModel()
        {
            FlorenceProperties = new List<ForeclosureModel>();
            DarlingtonProperties = new List<ForeclosureModel>();
            HorryProperties = new List<ForeclosureModel>();
            CharlestonProperties = new List<ForeclosureModel>();
        }

        public IList<ForeclosureModel> FlorenceProperties { get; set; }
        public IList<ForeclosureModel> DarlingtonProperties { get; set; }
        public IList<ForeclosureModel> HorryProperties { get; set; }
        public IList<ForeclosureModel> CharlestonProperties { get; set; }
    }
}