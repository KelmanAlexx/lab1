using System;
using System.Collections.Generic;
using System.Linq;

enum Gender { Мужской, Женский }
enum RoleType { Доктор, Ученый, Бизнесс_Коуч, Электрик, Ремонтник, Шиномонтажник, Психолог, Программист, Учитель, Безработный, CEO, Полицейский, Партийный_Работник, Вор, Врач_Учестковый, Уролог, Теоретик, Порно_Звезда, Груммер, Ветеренар, Парикмахер, Барбер, Работник_фермы }
enum FactType { Местный_Сумасшедший, Играет_в_шахматы, Любит_фурри, Пердит_по_ночам, Боится_врачей, Боится_учителей, Боится_фурри, Боится_Полиции, Любит_трогать_траву, Поталогический_лжец, Боится_женщин, Боится_мужчин, Икает_от_испуга, Не_может_без_манги, Повышенное_Потение, Пропагандирует_дыхание_маткой, Рыгает_при_всех, Боится_ходить_в_туалет_один, Ходит_в_туалет_по_2_часа, Не_может_спать_без_света, Боится_монстров_в_шкафу, Уверен_что_Фурри_существуют, Уверен_что_Пришельцы_реальны_и_они_нас_всех_спасут, Торгует_рулонами_с_газоном, Потомственный_ЖидоЕврей, Сбежал_с_работы_на_ферме, Работал_на_ферме, Был_ассистентом_врача, Пепрдавал_в_оксфорде, Любит_животных, Професианал_в_вскрытии_замков, Бывший_военный, Подрабатывает_на_стройке, Подрабатывает_в_кафе_горничных }

abstract class Person
{
    public string FullName { get; }
    public int Age { get; }
    public Gender Gender { get; }

    public Person(string fullName, int age, Gender gender)
    {
        FullName = fullName;
        Age = age;
        Gender = gender;
    }

    public abstract void Describe();
}

interface Votable
{
    void AddVote();
    int VoteCount { get; }
}

class PlayerGenerationException : Exception
{
    public PlayerGenerationException(string message) : base(message) { }

    public override string Message
    {
        get
        {
            return $"[Error] - Ошибка генерации игрока: {base.Message}";
        }
    }

}

record PlayerRecord(string FullName, int Age, RoleType Role, FactType Fact, Gender Gender, int ID);


class Player : Person, Votable
{
    public RoleType Role { get; }
    public FactType Fact { get; }
    public int ID { get; }
    public bool IsAlive { get; private set; } = true;
    public int VoteCount { get; private set; } = 0;

    public Player(string fullName, int age, RoleType role, FactType fact, Gender gender, int id)
        : base(fullName, age, gender)
    {
        Role = role;
        Fact = fact;
        ID = id;
    }

    public override void Describe()
    {
        Console.WriteLine($"{FullName} (Пол: {Gender}, Возраст: {Age}, Роль: {Role.ToString().Replace('_', ' ')}, Факт: {Fact.ToString().Replace('_', ' ')})");
    }

    public void AddVote(){
        VoteCount++;
    }
    public void ResetVote(){
        VoteCount = 0;
    }
    public void Kill(){
        IsAlive = false;
    }

    public override string ToString()
    {
        return $"{FullName} (Пол: {Gender}, Возраст: {Age}, Роль: {Role.ToString().Replace('_', ' ')}, Факт: {Fact.ToString().Replace('_', ' ')})";
    }
}

class Program
{
    static Random rnd = new Random();
    static List<Player> players = new List<Player>();
    static int nextID = 1;
    
    static void GeneratePlayers(int count)
    {
        string[] maleNames = { "Алексей", "Даниил", "Игорь", "Кирилл", "Виктор", "Григорий", "Михаил", "Никита", "Степан", "Андрей" };
        string[] femaleNames = { "Анна", "Мария", "Екатерина", "Ольга", "Татьяна", "Анастасия", "Елена", "Ирина", "Светлана", "Наталья" };
        string[] maleSurnames = { "Иванов", "Смирнов", "Кузнецов", "Попов", "Васильев" };
        string[] femaleSurnames = { "Иванова", "Смирнова", "Кузнецова", "Попова", "Васильева" };

        for (int i = 0; i < count; i++)
        {
            try
            {
                Gender gender;
                if (rnd.Next(2) == 0)
                {
                    gender = Gender.Мужской;
                }
                else
                {
                    gender = Gender.Женский;
                }
                
                string firstName;
                string lastName;
                
                if (gender == Gender.Мужской)
                {
                    firstName = maleNames[rnd.Next(maleNames.Length)];
                    lastName = maleSurnames[rnd.Next(maleSurnames.Length)];
                    
                }
                else
                {
                    firstName = femaleNames[rnd.Next(femaleNames.Length)];
                    lastName = femaleSurnames[rnd.Next(femaleSurnames.Length)];
                }
                
                if (firstName == null || lastName == null) throw new PlayerGenerationException("Имя или фамилия не могут быть null");

                string fullName = $"{firstName} {lastName}";
                RoleType role = (RoleType)rnd.Next(Enum.GetNames(typeof(RoleType)).Length);
                FactType fact = (FactType)rnd.Next(Enum.GetNames(typeof(FactType)).Length);
                int age = rnd.Next(18, 60);

                players.Add(new Player(fullName, age, role, fact, gender, nextID++));
            }
            catch (PlayerGenerationException ex)
            {
                Console.WriteLine(ex.Message);
                i--;
            }
        }
    }

    static void GameRound()
    {
        if (players.Count <= 4)
        {
            Console.WriteLine("\n--- Игра окончена! ---");
            Console.WriteLine("Выжившие:");
            foreach (var p in players)
            {
                Console.WriteLine(p);
            }
            return;
        }

        Console.WriteLine($"\n--- Раунд ---");

        foreach (var p in players)
        {
            int voteIndex;
            do
            {
                voteIndex = rnd.Next(players.Count);
            } while (players[voteIndex].ID == p.ID);

            players[voteIndex].AddVote();
            Console.WriteLine($"{p.FullName} голосует за {players[voteIndex].FullName}");
        }
        
        Player kicked = null;
        int maxVotes = -1;

        foreach (var p in players)
        {
            if (p.VoteCount > maxVotes)
            {
                maxVotes = p.VoteCount;
                kicked = p;
            }
        }

        kicked.Kill();
        players.Remove(kicked);
        Console.WriteLine($"\nИгрок {kicked.FullName} выбывает!");
        
        foreach (var p in players)
        {
            p.ResetVote();
        }

        
        GameRound();
    }

    static void Main()
    {
        GeneratePlayers(8);

        Console.WriteLine("--- Начальная команда ---");
        
        foreach (var p in players)
        {
            p.Describe();
        }
        
        Console.WriteLine("\n--- Игра начинается ---");
        GameRound();
    }
}
