
    public class ResultModel
    {
        public int httpStatus { get; set; }
        public string? Message { get; set; }
    }

    public class ResultModel<T>
    {
        public ResultModel()
        {

        }
        public int httpStatus { get; set; }
        public string? Message { get; set; }
        public T Value { get; set; }

        public ResultModel(int httpStatus, string message)
        {
            this.httpStatus = httpStatus;
            this.Message = message;
        }

        public ResultModel(int httpStatus, string message, T data)
        {
            this.httpStatus = httpStatus;
            this.Message = message;
            this.Value = data;
        }
    }
