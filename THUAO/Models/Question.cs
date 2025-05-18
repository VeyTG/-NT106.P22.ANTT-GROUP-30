using System.Collections.Generic;

namespace ThuAo.Models  // hoặc namespace tùy bạn chọn
{
    public class Question
    {
        public string Content { get; set; }
        public List<string> Options { get; set; }
        public int CorrectIndex { get; set; }  // chỉ số câu trả lời đúng
    }
}
