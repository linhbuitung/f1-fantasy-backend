namespace F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels
{
    public class ConstructorApiDto
    {
        //constructorId,constructorRef,name,nationality,url
        public string ConstructorId { get; set; }

        public string Url { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }

        public static ConstructorApiDto FromCsvLine(string line)
        {
            // Remove any double quotes from the string
            line = line.Replace("\"", string.Empty);
            string?[] parts = line.Split(',');

            return new ConstructorApiDto
            {
                ConstructorId = parts[0],
                Url = parts[2],
                Name = parts[3],
                Nationality = parts[4]
            };
        }

        public override string ToString()
        {
            string result = ConstructorId + "," + Url + "," + Name + "," + Nationality;
            return result;
        }
    }
}