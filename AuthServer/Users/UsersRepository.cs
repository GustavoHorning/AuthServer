namespace AuthServer.Users
{
    public class UsersRepository
    {
        //Lista interna que armazena os usuários
        private List<User> Users { get; set; } = new List<User>();

        // Método para salvar um novo usuário
        public User Save(User user)
        {
            user.ID = Users.Count + 1; // Atribui um ID único com base na contagem atual
            Users.Add(user); // Adiciona o usuário à lista
            return user; // Retorna o usuário salvo
        }

        // Método para recuperar um usuário pelo ID
        public User GetById(long id)
        {
            return Users.FirstOrDefault(x => x.ID == id); // Retorna o primeiro usuário que corresponde ao ID ou null
        }

        // Método para recuperar todos os usuários
        public List<User> FindAll()
        {
            return Users; // Retorna a lista de usuários
        }
    }
}
