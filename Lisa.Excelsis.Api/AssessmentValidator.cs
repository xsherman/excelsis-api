﻿using Lisa.Common.WebApi;
using Newtonsoft.Json.Linq;

namespace Lisa.Excelsis.Api
{
    public class AssessmentValidator : Validator
    {
        protected override void ValidatePatch()
        {
            Ignore("id");
            Allow("studentName");
            Allow("studentNumber");
            Allow("crebo");
            Allow("cohort");
            Allow("examSubject");
            Allow("examName");
            Allow("assessors");
            Allow("assessed");
            Allow("observations");
        }
        private void ValidateList(string fieldName, object value)
        {
            string[] expectedValues;
            string allowed;

            if (fieldName.ToLower() == "observations")
            {
                expectedValues = new string[] { "seen", "not seen", "not rated" };
                allowed = "seen, not seen and not rated";
            }
            else
            {
                expectedValues = new string[] { "excellent", "pass", "fail" };
                allowed = "excellent, pass and fail";
            }

            if ((value == null) ||
                (value is JArray) && ((JArray)value).Count == 0)
            {
                var error = new Error
                {
                    Code = ErrorCode.EmptyValue,
                    Message = $"The field '{fieldName.ToLowerInvariant()}' should not be empty",
                    Values = new
                    {
                        Field = fieldName.ToLowerInvariant()
                    }
                };
                Result.Errors.Add(error);
                return;
            }
            
            if (value.GetType().Name != "JArray")
            {
                var error = new Error
                {
                    Code = 10,
                    Message = $"The field '{fieldName.ToLowerInvariant()}' doesn't expects the value '{value}' in it's array. Only '{allowed}' allowed.",
                    Values = new
                    {
                        Field = fieldName.ToLowerInvariant()
                    }
                };
                Result.Errors.Add(error);
                return;
            }

            foreach(string val in (JArray)value)
            {
                bool expected = false;
                foreach(string expectedValue in expectedValues)
                {
                    if (val == expectedValue)
                    {
                        expected = true;
                    }
                }
                if (expected == false)
                {
                    var error = new Error
                    {
                        Code = 10,
                        Message = $"The field '{fieldName.ToLowerInvariant()}' doesn't expects the value '{val}' in it's array. Only {allowed} allowed.",
                        Values = new
                        {
                            Field = fieldName.ToLowerInvariant()
                        }
                    };
                    Result.Errors.Add(error);
                }
            }
        }
        protected override void ValidateModel()
        {
            Ignore("id");
            Optional("studentName", NotEmpty);
            Optional("studentNumber", NotEmpty, Length(8));
            Optional("crebo", NotEmpty, Length(5));
            Optional("cohort", NotEmpty, Length(4));
            Required("examSubject", NotEmpty);
            Required("examName", NotEmpty);
            Optional("assessors", NotEmpty);
            Optional("assessed", NotEmpty);
            Optional("observations", ValidateList);
            Required("norm", NotEmpty);
            Required("ratings", ValidateList);
        }
    }
}