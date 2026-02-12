using System;

namespace HealthConnect.Models.Dto
{
    public class DoctorUpdateProfileDto
    {
        public string Name { get; set; }
       
        public int YearsOfExperience { get; set; }
        public string Bio { get; set; }
       
        public string PhoneNumber { get; set; }
    }
}
