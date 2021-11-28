namespace API_Tests_TaskBoard
{
    public class TaskResponse
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public BoardModel board { get; set; }
    }

    public class BoardModel 
    {
        public int id { get; set; }
        public string name { get; set; }
    }

}