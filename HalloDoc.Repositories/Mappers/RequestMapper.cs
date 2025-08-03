using HalloDoc.Common.Constants;
using HalloDoc.Entities.Data.Entities;
using HalloDoc.Repositories.DTOs;

namespace HalloDoc.Repositories.Mappers
{
    public static class RequestMapper
    {
        public static Request ToRequestEntity(RequestCreateDto dto)
        {
            return new Request
            {
                RequestType = dto.RequestType,
                RequestorType = dto.RequestorType,
                RequestorFirstName = dto.RequestorFirstName,
                RequestorLastName = dto.RequestorLastName,
                RequestorEmail = dto.RequestorEmail,
                RequestorPhone = dto.RequestorPhone,
                RelationWithPatient = dto.RelationWithPatient,
                HotelName = dto.HotelName,
                PropertyName = dto.PropertyName,
                CaseNumber = dto.CaseNumber,
                PatientId = dto.PatientId,
                Symptoms = dto.Symptoms,
                CreatedAt = DateTime.Now,
                RequestStatus = (int)RequestStatus.Unassigned // Default to Unassigned
            };
        }

        public static RequestClient ToRequestClientEntity(RequestCreateDto dto)
        {
            return new RequestClient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DOB = dto.DOB,
                Email = dto.Email,
                Phone = dto.Phone,
                Street = dto.Street,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                RoomNo = dto.RoomNo,
                UserId = dto.UserId
            };
        }
    }
} 