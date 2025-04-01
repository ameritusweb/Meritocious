namespace Meritocious.Blazor.Pages.Auth
{
    public class RegisterModel
    {
        public string Email { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
    }

    public class ProfileModel
    {
        public string DisplayName { get; set; } = "";
        public string Bio { get; set; } = "";
        public string? AvatarUrl { get; set; }
        public string? AvatarFile { get; set; }
    }

    public class InterestsModel
    {
        public List<string> Topics { get; set; } = new();
    }

    public class TopicCategory
    {
        public string Name { get; set; }
        public List<Topic> Topics { get; set; }

        public TopicCategory(string name, List<Topic> topics)
        {
            Name = name;
            Topics = topics;
        }
    }

    public class Topic
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Topic(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class ContentPreference
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public decimal Value { get; set; }

        public ContentPreference(string key, string label, decimal value)
        {
            Key = key;
            Label = label;
            Value = value;
        }
    }
}
