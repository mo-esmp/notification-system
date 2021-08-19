using System.Collections.Generic;
using System.Linq;

namespace WebClient.Models
{
    public class ResponseBase
    {
        public ResponseBase()
        {
        }

        public bool Succeeded => !Errors?.Any() ?? true;

        public IDictionary<string, IEnumerable<string>> Errors { get; set; }

        public string GetErrorMessages => Errors != null
            ? string.Join("<br/>", Errors.SelectMany(e => e.Value))
            : string.Empty;

        public static T CreateInternalServerError<T>() where T : ResponseBase, new()
        {
            return new()
            {
                Errors = new Dictionary<string, IEnumerable<string>>
                {
                    {"ServerError", new List<string> {"Internal server error"}}
                }
            };
        }

        public static T CreateUnAuthorizedError<T>() where T : ResponseBase, new()
        {
            return new()
            {
                Errors = new Dictionary<string, IEnumerable<string>>
                {
                    {"ServerError", new List<string> {"Unauthorized access error"}}
                }
            };
        }

        public static T CreateBadRequestError<T>() where T : ResponseBase, new()
        {
            return new()
            {
                Errors = new Dictionary<string, IEnumerable<string>>
                {
                    {"ServerError", new List<string> {"Bad request error"}}
                }
            };
        }
    }

    public class HttpError
    {
        public IEnumerable<IDictionary<string, IEnumerable<string>>> Errors { get; set; }
    }
}