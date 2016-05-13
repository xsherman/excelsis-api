﻿using Lisa.Common.WebApi;

namespace Lisa.Excelsis.Api
{
    public class AssessmentValidator : Validator
    {
        protected override void ValidatePatch()
        {
            Ignore("id");
            Allow("student.name");
            Allow("student.number");
            Allow("crebo");
            Allow("cohort");
            Allow("subject");
            Allow("exam");
            Allow("assessors");
            Allow("assessed");
            Allow("observations");
        }

        protected override void ValidateModel()
        {
            Ignore("id");
            Optional("student.name", NotEmpty);
            Optional("student.number", NotEmpty);
            Optional("crebo", NotEmpty, Length(5));
            Optional("cohort", NotEmpty, Length(4));
            Required("subject", NotEmpty);
            Required("exam", NotEmpty);
            //Optional("assessors", NotEmpty, IsArray(DataTypes.Object));
            Optional("assessors.firstName", NotEmpty, TypeOf(DataTypes.String));
            Optional("assessors.lastName", NotEmpty, TypeOf(DataTypes.String));
            Optional("assessors.userName", NotEmpty, TypeOf(DataTypes.String));
            Optional("assessors.teacherCode", NotEmpty, TypeOf(DataTypes.String));
            Optional("assessed", NotEmpty);
            Optional("observations.value", NotEmpty, TypeOf(DataTypes.String), OneOf("notRated", "notSeen", "seen"));
            Optional("observations.remark", NotEmpty, TypeOf(DataTypes.String));
            Optional("observations.markings", NotEmpty, IsArray(DataTypes.String));
            Required("observations.criteria.description", NotEmpty, TypeOf(DataTypes.String));
            Required("observations.criteria.details", NotEmpty, TypeOf(DataTypes.String));
            Required("observations.criteria.rating", NotEmpty, TypeOf(DataTypes.String), OneOf("fail", "pass", "excellent"));
            Required("observations.criteria.category", NotEmpty, TypeOf(DataTypes.String));
            Required("norm.excellent", NotEmpty, IsArray(DataTypes.Number));
            Required("norm.pass", NotEmpty, IsArray(DataTypes.Number));
            Required("norm.fail", NotEmpty, IsArray(DataTypes.Number));
        }
    }
}