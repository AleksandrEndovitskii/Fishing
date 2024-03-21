namespace Models
{
    public class PlayerModel : IModel
    {
        public ulong OwnerClientId
        {
            get;
            private set;
        }
    }
}
