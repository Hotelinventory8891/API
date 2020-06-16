using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Translators
{
    public interface IEntityTranslator
    {
        bool CanTranslate<TTarget, TSource>();
        bool CanTranslate(Type targetType, Type sourceType);
        TTarget Translate<TTarget>(IEntityTranslatorService service, object source);
        object Translate(IEntityTranslatorService service, Type targetType, object source);
    }
}
