namespace yocar.Insurance.Entities
{
    public class DataCCCDEntity
    {
        public class DataCCCD
        {
            public string IdNumber { get; set; }
            public string Name { get; set; }
            public string DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string Address { get; set; }
            public string DateOfIssue { get; set; }
        }

        public class DataItemFront
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Dob { get; set; }
            public string Sex { get; set; }
            public string Nationality { get; set; }
            public string Home { get; set; }
            public string Address { get; set; }
        }

        public class ApiResponseFront
        {
            public int ErrorCode { get; set; }
            public string ErrorMessage { get; set; }
            public List<DataItemFront> Data { get; set; }
        }

        public class ApiResponseBackside
        {
            public int ErrorCode { get; set; }
            public string ErrorMessage { get; set; }
            public List<DataItemBackside> Data { get; set; }
        }

        public class DataItemBackside
        {
            public string Features { get; set; }
            public string Issue_Date { get; set; }
        }

        public static object CompareDataCCCDs(DataCCCD obj1, DataCCCD obj2)
        {
            if (obj1 == null || obj2 == null)
                return "One or both objects are null.";

            var errors = new List<string>();

            if (!string.Equals(obj1.IdNumber, obj2.IdNumber, StringComparison.OrdinalIgnoreCase))
                errors.Add("IdNumber does not match.");

            if (!string.Equals(obj1.Name, obj2.Name, StringComparison.OrdinalIgnoreCase))
                errors.Add("Name does not match.");

            if (!string.Equals(obj1.DateOfBirth, obj2.DateOfBirth, StringComparison.OrdinalIgnoreCase))
                errors.Add("DateOfBirth does not match.");

            if (!string.Equals(obj1.Gender, obj2.Gender, StringComparison.OrdinalIgnoreCase))
                errors.Add("Gender does not match.");

            if (!string.Equals(obj1.Address, obj2.Address, StringComparison.OrdinalIgnoreCase))
                errors.Add("Address does not match.");

            if (!string.Equals(obj1.DateOfIssue, obj2.DateOfIssue, StringComparison.OrdinalIgnoreCase))
                errors.Add("DateOfIssue does not match.");

            return errors.Count > 0 ? new { Status = "Failed", Error = string.Join(", ", errors) } : new { Status = "Success" };
        }
    }

}
