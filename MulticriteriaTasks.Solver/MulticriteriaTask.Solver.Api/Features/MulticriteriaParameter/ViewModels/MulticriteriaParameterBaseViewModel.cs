using AutoMapper;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTask.ViewModels;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskParameterHeader.ViewModels;
using MulticriteriaTask.Solver.Db.Models;
using System;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaParameter.ViewModels
{
    public class MulticriteriaParameterBaseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public double Value { get; set; }

        public double? PolarAngle { get; set; }
        public double? RadiansAngle { get; set; }
        public string DecartCoordinate { get; set; }

        public MulticriteriaTaskParameterHeaderBaseViewModel Header { get; set; }

        public MulticriteriaTaskBaseViewModel MulticriteriaTask { get; set; }
    }

    public class MulticriteriaParameterBaseViewModelProfiler : Profile
    {
        public MulticriteriaParameterBaseViewModelProfiler()
        {
            CreateMap<MulticriteriaParameterModel, MulticriteriaParameterBaseViewModel>()
                .ForMember(desc => desc.Header, opt => opt.MapFrom(src => src.Header))
                .ForMember(desc => desc.MulticriteriaTask, opt => opt.MapFrom(src => src.MulticriteriaTask));
        }
    }
}