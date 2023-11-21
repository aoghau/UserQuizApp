namespace UserQuizApp.Interfaces
{
    public interface IAuthService
    {
        public bool ValidateUser();
        public string ValidatedUserName();
    }
}
