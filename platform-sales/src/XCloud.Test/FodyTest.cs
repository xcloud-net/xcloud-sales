using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XCloud.Test;

[TestClass]
public class FodyTest
{
    class User
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public User Boy { get; set; }
    }

    [TestMethod]
    public void tostring_test()
    {
        var data = new User()
        {
            Name = "fad",
            Age = 12,
            Boy = new User() { }
        }.ToString();
    }
}