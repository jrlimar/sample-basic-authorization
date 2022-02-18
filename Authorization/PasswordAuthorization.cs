using System;

namespace sample_basic_authorization.Authorization
{
    public class PasswordAuthorization
    {
        private readonly string apiPassword;

        public PasswordAuthorization(string apiPassword)
        {
            this.apiPassword = apiPassword;
        }

        public bool IsValid(string passwordToTest)
        {
            try
            {
                return passwordToTest.Equals(apiPassword);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
