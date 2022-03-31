using AutoMapper;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaParameter.ViewModels;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskType.ViewModels;
using MulticriteriaTask.Solver.Db.Enums;
using MulticriteriaTask.Solver.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTask.ViewModels
{
    public class MulticriteriaTaskBaseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double? Squere { get; set; }
        public double? IntersectionSquere { get; set; }

        public MulticriteriaTaskStatusEnum Status { get; set; }

        public MulticriteriaTaskTypeBaseViewModel Type { get; set; }

        public MulticriteriaParameterBaseViewModel[] MulticriteriaTaskParameters { get; set; }
    }

    public class MulticriteriaParameterBaseViewModelProfiler : Profile
    {
        public MulticriteriaParameterBaseViewModelProfiler()
        {
            CreateMap<MulticriteriaTaskModel, MulticriteriaTaskBaseViewModel>()
                .ForMember(desc => desc.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(desc => desc.MulticriteriaTaskParameters, opt => opt.MapFrom(src => src.Parameters));
        }
    }
}
