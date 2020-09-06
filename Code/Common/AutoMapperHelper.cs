using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Bingo.Common
{
    /// <summary>
    ///     AutoMapper扩展帮助类
    /// </summary>
    public static class AutoMapperHelper
    {
        public static IMapper DMapper = DynamicMap();
    
        /// <summary>
        ///     类型映射
        /// </summary>
        public static T MapTo<T>(this object obj)
        {
            if (obj == null) return default(T);
            AutoMapper.Mapper.Map(obj.GetType(), typeof(T));
            return AutoMapper.Mapper.Map<T>(obj);
        }

        /// <summary>
        ///     类型映射
        /// </summary>
        public static T DMapTo<T>(this object obj)
        {
            if (obj == null) return default(T);
            DMapper.Map(obj.GetType(), typeof(T));
            return DMapper.Map<T>(obj);
        }



        /// <summary>
        ///     Dto映射到实体对象
        /// </summary>
        public static T MapToEntity<T>(this object obj, T entityObj)
        {
            AutoMapper.Mapper.Map(obj.GetType(), typeof(T));
            return AutoMapper.Mapper.Map(obj, entityObj);
        }

        public static T DMapToEntity<T>(this object obj, T entityObj)
        {
            DMapper.Map(obj.GetType(), typeof(T));
            return DMapper.Map(obj, entityObj);
        }

        /// <summary>
        ///     集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TDestination>(this IEnumerable source)
        {
            foreach (var first in source)
            {
                var type = first.GetType();
                AutoMapper.Mapper.Map(type, typeof(TDestination));
                break;
            }

            return AutoMapper.Mapper.Map<List<TDestination>>(source);
        }

        public static List<TDestination> DMapToList<TDestination>(this IEnumerable source)
        {
            foreach (var first in source)
            {
                var type = first.GetType();
                DMapper.Map(type, typeof(TDestination));
                break;
            }

            return DMapper.Map<List<TDestination>>(source);
        }

        /// <summary>
        ///     集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            //IEnumerable<T> 类型需要创建元素的映射
            //Mapper.Map<TSource, TDestination>();
            return AutoMapper.Mapper.Map<List<TDestination>>(source);
        }

        public static List<TDestination> DMapToList<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            //IEnumerable<T> 类型需要创建元素的映射
            //Mapper.Map<TSource, TDestination>();
            return DMapper.Map<List<TDestination>>(source);
        }

        /// <summary>
        ///     类型映射
        /// </summary>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            if (source == null) return destination;
            //Mapper.Map<TSource, TDestination>();
            return AutoMapper.Mapper.Map(source, destination);
        }

        public static TDestination DMapTo<TSource, TDestination>(this TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            if (source == null) return destination;
            //Mapper.Map<TSource, TDestination>();
            return DMapper.Map(source, destination);
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public static void Init()
        {
            //定义映射方式
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<CommonMapper>();
                cfg.ForAllMaps((a, b) => b.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null)));
                
            });
            //Mapper.AssertConfigurationIsValid();
        }

        /// <summary>
        ///     对象对对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IMapper DynamicMap()
        {
            var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMissingTypeMaps = true;
                    cfg.AllowNullCollections = true;
                    cfg.AllowNullDestinationValues = true;
                    cfg.EnableNullPropagationForQueryMapping = true;
                    cfg.ValidateInlineMaps = false;
                    cfg.ForAllMaps((map, exp) => exp.ForAllOtherMembers(opt => opt.Ignore()));
                }
            );
            return config.CreateMapper();
        }

        ///// <summary>
        /////     DataReader映射
        ///// </summary>
        //public static IEnumerable<T> DataReaderMapTo<T>(this IDataReader reader)
        //{
        //    Mapper.Map<IDataReader, IEnumerable<T>>();
        //    return Mapper.Map<IDataReader, IEnumerable<T>>(reader);
        //}
    }

    //public static class AutoMapperConfig
    //{
    //    public static IMapper Mapper = new AutoMapper.Mapper(RegisterMappings());

    //    public static MapperConfiguration RegisterMappings()
    //    {
    //        return new MapperConfiguration(cfg =>
    //        {
    //            cfg.CreateMissingTypeMaps = true;
    //            cfg.AllowNullDestinationValues = false;
    //            cfg.ForAllPropertyMaps(IsToRepeatedField, (propertyMap, opts) => opts.UseDestinationValue());
    //            cfg.ForAllPropertyMaps(IsToMapFieldField, (propertyMap, opts) => opts.UseDestinationValue());
    //        });

    //        bool IsToRepeatedField(PropertyMap pm)
    //        {
    //            if (pm.DestinationPropertyType.IsConstructedGenericType)
    //            {
    //                var destGenericBase = pm.DestinationPropertyType.GetGenericTypeDefinition();
    //                return false; // destGenericBase == typeof(RepeatedField<>);
    //            }

    //            return false;
    //        }

    //        bool IsToMapFieldField(PropertyMap pm)
    //        {
    //            if (pm.DestinationPropertyType.IsConstructedGenericType)
    //            {
    //                var destGenericBase = pm.DestinationPropertyType.GetGenericTypeDefinition();
    //                return false; // destGenericBase == typeof(MapField<,>);
    //            }

    //            return false;
    //        }
    //    }
    //}
}