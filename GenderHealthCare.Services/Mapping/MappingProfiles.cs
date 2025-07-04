﻿using AutoMapper;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.ConsultantModel;
using GenderHealthCare.ModelViews.ConsultantScheduleModel;
using GenderHealthCare.ModelViews.TestBookingModel;
using GenderHealthCare.ModelViews.TestSlotModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Services.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            /* ---------------- ConsultantSchedule <‑‑> DTO ---------------- */

            CreateMap<ConsultantSchedule, ConsultantScheduleDto>()
                .ForMember(dest => dest.ConsultantName,
                           opt => opt.MapFrom(src => src.Consultant.User.FullName))
                .ReverseMap()
                .ForMember(dest => dest.Consultant, opt => opt.Ignore())
                .ForMember(dest => dest.Slots, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<CreateConsultantScheduleDto, ConsultantSchedule>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Consultant, o => o.Ignore())
            .ForMember(d => d.Slots, o => o.Ignore());

            CreateMap<UpdateConsultantScheduleDto, ConsultantSchedule>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Consultant, o => o.Ignore())
            .ForMember(d => d.Slots, o => o.Ignore());

            /* ---------------- Consultant <‑‑> DTO ---------------- */

            CreateMap<Consultant, ConsultantDto>().ReverseMap();
            CreateMap<CreateConsultantDto, Consultant>()
            .ForMember(dest => dest.User,
                     opt => opt.MapFrom(src => new User
                     {
                         FullName = src.FullName,
                         Email = src.Email,
                         PhoneNumber = src.PhoneNumber,
                         DateOfBirth = src.DateOfBirth,
                         Gender = src.Gender,
                         Role = "Consultant",
                         IsCycleTrackingOn = src.IsCycleTrackingOn
                     }));
            CreateMap<UpdateConsultantDto, Consultant>()
            .ForAllMembers(opt => opt.Condition(
                (src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateConsultantDto, User>()
                .ForMember(d => d.Role, o => o.Ignore())
                .ForMember(d => d.PasswordHash, o => o.Ignore())
                .ForAllMembers(opt => opt.Condition(
                    (src, dest, srcMember) => srcMember != null));


            CreateMap<TestSlot, TestSlotDto>();
            CreateMap<CreateTestSlotDto, TestSlot>();
            CreateMap<UpdateTestSlotDto, TestSlot>();

            CreateMap<TestBooking, TestBookingDto>();
            CreateMap<CreateTestBookingDto, TestBooking>();
            CreateMap<UpdateTestBookingDto, TestBooking>();

        }
    }
}
