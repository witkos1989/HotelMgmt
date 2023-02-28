using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Infrastructure.Services
{
    public class ModelStateHelpers
    {
        public static string SerialiseModelState(ModelStateDictionary modelState)
        {
            var errorList = modelState
                .Select(el => new ModelStateTransferValue
                {
                    Key = el.Key,
                    AttemptedValue = el.Value!.AttemptedValue!,
                    RawValue = el.Value.RawValue!,
                    ErrorMessages = el.Value.Errors.Select(err => err.ErrorMessage).ToList(),
                });

            return JsonConvert.SerializeObject(errorList);
        }

        public static ModelStateDictionary DeserialiseModelState(string serialisedErrorList)
        {
            var errorList = JsonConvert.DeserializeObject<List<ModelStateTransferValue>>(serialisedErrorList);
            var modelState = new ModelStateDictionary();
            if (errorList is not null)
                foreach (var item in errorList)
                {
                    modelState.SetModelValue(item.Key, item.RawValue, item.AttemptedValue);
                    foreach (var error in item.ErrorMessages)
                    {
                        modelState.AddModelError(item.Key, error);
                    }
                }
            return modelState;
        }
    }

#pragma warning disable CS8618
    public class ModelStateTransferValue
    {
        public string Key { get; set; }

        public string AttemptedValue { get; set; }

        public object RawValue { get; set; }

        public ICollection<string> ErrorMessages { get; set; } = new List<string>();
    }
#pragma warning restore CS8618
}