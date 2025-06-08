using System.Globalization;

namespace F1Fantasy.Infrastructure.ExternalServices.TempModels
{
    public class DriverApiDto
    {
        // Matches the API response structure
        public string DriverId { get; set; } // "driverId"

        public string Url { get; set; } // "url"

        public string GivenName { get; set; } // "givenName"

        public string FamilyName { get; set; } // "familyName"

        public DateTime DateOfBirth { get; set; } // "dateOfBirth"

        public string Nationality { get; set; } // "nationality"

        public static DriverApiDto FromApiResponse(dynamic apiResponse)
        {
            return new DriverApiDto
            {
                DriverId = apiResponse.driverId,
                Url = apiResponse.url,
                GivenName = apiResponse.givenName,
                FamilyName = apiResponse.familyName,
                DateOfBirth = DateTime.Parse(apiResponse.dateOfBirth, CultureInfo.InvariantCulture),
                Nationality = apiResponse.nationality
            };
        }

        public override string ToString()
        {
            return $"{DriverId},{GivenName},{FamilyName},{DateOfBirth},{Nationality},{Url}";
        }
    }
}