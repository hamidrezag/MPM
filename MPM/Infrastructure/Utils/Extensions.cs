
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Utils;

namespace Infrastructure.Extensions
{
    public static class Extention
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        //public static string GetDescription(this Enum @enum) => @enum.Description();

        public static PropertyInfo ToPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);
            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
            {
                var ubody = (UnaryExpression)propertyLambda.Body;
                member = ubody.Operand as MemberExpression;
                if (member == null)
                    throw new ArgumentException(string.Format(
                        "Expression '{0}' refers to a method, not a property.",
                        propertyLambda.ToString()));
            }
            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }
        public static IQueryable<T> Pagination<T, TKey>(this IQueryable<T> query, int pageSize, int pageNumber, Expression<Func<T, dynamic>> orderField = null, bool ascSorted = true)
            where T : class, IBaseModel<TKey>
        {

            if (orderField == null)
                return Paginate(query, pageSize, pageNumber, x => x.Id, ascSorted);
            else
                return Paginate(query, pageSize, pageNumber, orderField, ascSorted);
        }
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageSize, int pageNumber, Expression<Func<T, dynamic>> orderField, bool ascSorted = true)
        {
            query = ascSorted ? query.OrderBy(orderField) : query.OrderByDescending(orderField);
            var skip = ((pageNumber <= 1 ? 1 : pageNumber) - 1) * pageSize;
            return query.Skip(skip).Take(pageSize);
        }
        //public static DateTime ToSystemDate(this string persianDate)
        //{
        //    PersianCalendar pc = new PersianCalendar();
        //    persianDate = persianDate.ChangePersianNumbersToEnglish();
        //    int year = int.Parse(persianDate.Substring(0, 4));
        //    int month = int.Parse(persianDate.Substring(5, 2));
        //    int day = int.Parse(persianDate.Substring(8, 2));
        //    return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
        //}
        //public static DateTime ToSystemDateWithoutSplit(this string persianDate)
        //{
        //    PersianCalendar pc = new PersianCalendar();
        //    persianDate = persianDate.ChangePersianNumbersToEnglish();
        //    int year = int.Parse(persianDate.Substring(0, 4));
        //    int month = int.Parse(persianDate.Substring(4, 2));
        //    int day = int.Parse(persianDate.Substring(6, 2));
        //    return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
        //}
        //public static DateTime ToSystemDateTime(this string persianDateTime)
        //{
        //    string[] splited = persianDateTime.Split(' ');
        //    string persianDate = splited[0];
        //    PersianCalendar pc = new PersianCalendar();
        //    persianDate = persianDate.ChangePersianNumbersToEnglish();
        //    int year = int.Parse(persianDate.Substring(0, 4));
        //    int month = int.Parse(persianDate.Substring(5, 2));
        //    int day = int.Parse(persianDate.Substring(8, 2));
        //    string persianTime = splited[1];
        //    persianTime = persianTime.ChangePersianNumbersToEnglish();
        //    splited = persianTime.Split(":");
        //    int hour = Convert.ToInt32(splited[0]);
        //    int minute = Convert.ToInt32(splited[1]);
        //    return pc.ToDateTime(year, month, day, hour, minute, 0, 0);
        //}
        public static string ChangePersianNumbersToEnglish(this string input)
        {
            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            for (int j = 0; j < persian.Length; j++)
                input = input.Replace(persian[j], j.ToString());

            return input;
        }
        //public static string ToPersianDate(this DateTime date)
        //{
        //    PersianCalendar pc = new PersianCalendar();
        //    return string.Format("{0}/{1}/{2}", pc.GetYear(date), pc.GetMonth(date).ToString().PadLeft(2,'0'), pc.GetDayOfMonth(date).ToString().PadLeft(2,'0'));
        //}

        //public static string ToPersianDateTime(this DateTime date)
        //{
        //    PersianCalendar pc = new PersianCalendar();
        //    return string.Format("{0}/{1}/{2} {3}:{4}:{5}", pc.GetYear(date), pc.GetMonth(date), pc.GetDayOfMonth(date), date.Hour, date.Minute, date.Second);
        //}
        public static string? GetUserId(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }


        public static IList<T> SqlQuery<T>(this DbContext db, string sql, params object[] parameters) where T : class
        {
            using (var db2 = new ContextForQueryType<T>(db.Database.GetDbConnection()))
            {
                return db2.Set<T>().FromSqlRaw(sql, parameters).ToList();
            }
        }

        private class ContextForQueryType<T> : DbContext where T : class
        {
            private readonly DbConnection connection;

            public ContextForQueryType(DbConnection connection)
            {
                this.connection = connection;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(connection, options => options.EnableRetryOnFailure());

                base.OnConfiguring(optionsBuilder);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<T>().HasNoKey();
                base.OnModelCreating(modelBuilder);
            }
        }


        public static bool IsValidNationalCode(this string nationalCode)
        {
            //در صورتی که کد ملی وارد شده تهی باشد

            if (String.IsNullOrEmpty(nationalCode))
                //throw new Exception("لطفا کد ملی را صحیح وارد نمایید");
                return false;


            //در صورتی که کد ملی وارد شده طولش کمتر از 10 رقم باشد
            if (nationalCode.Length != 10)
                //throw new Exception("طول کد ملی باید ده کاراکتر باشد");
                return false;

            //در صورتی که کد ملی ده رقم عددی نباشد
            var regex = new Regex(@"\d{10}");
            if (!regex.IsMatch(nationalCode))
                //throw new Exception("کد ملی تشکیل شده از ده رقم عددی می‌باشد؛ لطفا کد ملی را صحیح وارد نمایید");
                return false;

            //در صورتی که رقم‌های کد ملی وارد شده یکسان باشد
            var allDigitEqual = new[] { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
            if (allDigitEqual.Contains(nationalCode)) return false;


            //عملیات شرح داده شده در بالا
            var chArray = nationalCode.ToCharArray();
            var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
            var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
            var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
            var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
            var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
            var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
            var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
            var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
            var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
            var a = Convert.ToInt32(chArray[9].ToString());

            var b = (((((((num0 + num2) + num3) + num4) + num5) + num6) + num7) + num8) + num9;
            var c = b % 11;

            return (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a)));

        }


        public static IRuleBuilderOptionsConditions<T, string> IsNationalCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((national, context) =>
            {
                var chArray = national.ToCharArray();
                var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
                var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
                var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
                var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
                var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
                var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
                var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
                var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
                var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
                var a = Convert.ToInt32(chArray[9].ToString());

                var b = (((((((num0 + num2) + num3) + num4) + num5) + num6) + num7) + num8) + num9;
                var c = b % 11;

                var res = (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a)));

                if (!res)
                    context.AddFailure("Wrong National Code");
            });
        }

        public static IRuleBuilderOptionsConditions<T, string> NotEqual<T>(this IRuleBuilder<T, string> ruleBuilder, string[] strs)
        {
            return ruleBuilder.Custom((str, context) =>
            {
                foreach (var item in strs)
                    if (item == str)
                        context.AddFailure("item exist in array");
            });
        }


        public static PersianDateFormat PersianDate(this DateTime dt)
        {
            if (String.IsNullOrEmpty(dt.ToString()))
            {
                dt = DateTime.Now;
            }
            try
            {
                PersianCalendar pc = new PersianCalendar();
                var dayOfWeek = ShamsiDayOfWeek(dt);
                var dayOfmonth = pc.GetDayOfMonth(dt);
                var hour = pc.GetHour(dt);
                var second = pc.GetSecond(dt);
                var minute = pc.GetMinute(dt);
                var month = pc.GetMonth(dt);
                var year = pc.GetYear(dt);

                PersianDateFormat Date = new PersianDateFormat
                {
                    Day = dayOfmonth.ToString("00"),
                    Time = hour.ToString("00") + ":" + minute.ToString("00"),
                    Month = month.ToString("00"),
                    Year = year.ToString(),
                    Hour = hour.ToString("00"),
                    Minute = minute.ToString("00"),
                    Second = second.ToString("00"),
                    DateNumber = Convert.ToInt32((year.ToString("00") + month.ToString("00") + dayOfmonth.ToString("00")).TrimStart()),
                    MonthName = MonthName(month),
                    DayOfWeek = dayOfWeek.ToString(),
                    Date = year + "/" + month.ToString("00") + "/" + dayOfmonth.ToString("00"),
                    DateTime = year + "/" + month.ToString("00") + "/" + dayOfmonth.ToString("00") + " " + hour.ToString("00") + ":" + minute.ToString("00"),
                    DateDayName = dayOfWeek + " " + year + "/" + month.ToString("00") + "/" + dayOfmonth.ToString("00") + " " + hour + ":" + minute,
                    DateDayNameWithoutTime = dayOfWeek + " " + year + "/" + month.ToString("00") + "/" + dayOfmonth.ToString("00"),
                    DateNumberWithTime = Convert.ToInt64((
                    year.ToString() + month.ToString("00") + dayOfmonth.ToString("00") +
                    hour.ToString("00") + minute.ToString("00") + second.ToString("00")).TrimStart()),
                    DateNumberWithCompleteYear = Convert.ToInt64((year.ToString() + month.ToString("00") + dayOfmonth.ToString("00")).TrimStart()),
                };

                return Date;
            }
            catch
            {
                return null;
            }
        }
        public static string MonthName(int monthId)
        {
            switch (monthId)
            {
                case 1: return "فروردین";
                case 2: return "اردیبهشت";
                case 3: return "خرداد";
                case 4: return "تیر";
                case 5: return "مرداد";
                case 6: return "شهریور";
                case 7: return "مهر";
                case 8: return "آبان";
                case 9: return "آذر";
                case 10: return "دی";
                case 11: return "بهمن";
                case 12: return "اسفند";
            }
            return "";
        }
        public static string ShamsiDayOfWeek(DateTime dt)
        {
            string DayName = "";
            if (dt.DayOfWeek == DayOfWeek.Saturday)
                DayName = "شنبه";
            else if (dt.DayOfWeek == DayOfWeek.Sunday)
                DayName = "یکشنبه";
            else if (dt.DayOfWeek == DayOfWeek.Monday)
                DayName = "دوشنبه";
            else if (dt.DayOfWeek == DayOfWeek.Tuesday)
                DayName = "سه شنبه";
            else if (dt.DayOfWeek == DayOfWeek.Wednesday)
                DayName = "چهارشنبه";
            else if (dt.DayOfWeek == DayOfWeek.Thursday)
                DayName = "پنجشنبه";
            else if (dt.DayOfWeek == DayOfWeek.Friday)
                DayName = "جمعه";
            return DayName;
        }
        public static string to2Number(this int number)
        {
            string res = "";
            if (number > 9)
                res = number.ToString();
            else
                res = "0" + number.ToString();
            return res;
        }
        public static bool IsFromForm(this ApiParameterDescription apiParameter)
        {
            var source = apiParameter.Source;
            var elementType = apiParameter.ModelMetadata?.ElementType;

            return (source == BindingSource.Form || source == BindingSource.FormFile)
                || (elementType != null && typeof(IFormFile).IsAssignableFrom(elementType));
        }
    //    public static Expression<Func<T, bool>> AndAlso<T>(
    //this Expression<Func<T, bool>> leftex,
    //Expression<Func<T, bool>> rightex)
    //    {
    //        var param = Expression.Parameter(typeof(T), "x");
    //        var body = Expression.AndAlso(
    //                Expression.Invoke(leftex, param),
    //                Expression.Invoke(rightex, param)
    //            );
    //        var lambda = Expression.Lambda<Func<T, bool>>(body, param);
    //        return lambda;
    //        //var parameter = Expression.Parameter(typeof(T));

    //        //var leftVisitor = new ReplaceExpressionVisitor(leftex.Parameters[0], parameter);
    //        //var left = leftVisitor.Visit(leftex.Body);

    //        //var rightVisitor = new ReplaceExpressionVisitor(rightex.Parameters[0], parameter);
    //        //var right = rightVisitor.Visit(rightex.Body);

    //        //return Expression.Lambda<Func<T, bool>>(
    //        //    Expression.AndAlso(left, right), parameter);
    //    }
        
        //    private class ReplaceExpressionVisitor
        //    : ExpressionVisitor
        //    {
        //        private readonly Expression _oldValue;
        //        private readonly Expression _newValue;

        //        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        //        {
        //            _oldValue = oldValue;
        //            _newValue = newValue;
        //        }

        //        public override Expression Visit(Expression node)
        //        {
        //            if (node == _oldValue)
        //                return _newValue;
        //            return base.Visit(node);
        //        }
        //    }
        //public static Expression<Func<T, bool>> AndAlso<T>(
        //this Expression<Func<T, bool>> expr1,
        //Expression<Func<T, bool>> expr2)
        //{
        //    // need to detect whether they use the same
        //    // parameter instance; if not, they need fixing
        //    ParameterExpression param = expr1.Parameters[0];
        //    if (ReferenceEquals(param, expr2.Parameters[0]))
        //    {
        //        // simple version
        //        return Expression.Lambda<Func<T, bool>>(
        //            Expression.AndAlso(expr1.Body, expr2.Body), param);
        //    }
        //    // otherwise, keep expr1 "as is" and invoke expr2
        //    return Expression.Lambda<Func<T, bool>>(
        //        Expression.AndAlso(
        //            expr1.Body,
        //            Expression.Invoke(expr2, param)), param);
        //}
        //public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> leftExpression,
        //Expression<Func<T, bool>> rightExpression) =>
        //Combine(leftExpression, rightExpression, Expression.AndAlso);

        //public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> leftExpression,
        //    Expression<Func<T, bool>> rightExpression) =>
        //    Combine(leftExpression, rightExpression, Expression.Or);

        //public static Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> leftExpression, Expression<Func<T, bool>> rightExpression, Func<Expression, Expression, BinaryExpression> combineOperator)
        //{
        //    var leftParameter = leftExpression.Parameters[0];
        //    var rightParameter = rightExpression.Parameters[0];

        //    var visitor = new ReplaceParameterVisitor(rightParameter, leftParameter);

        //    var leftBody = leftExpression.Body;
        //    var rightBody = visitor.Visit(rightExpression.Body);

        //    return Expression.Lambda<Func<T, bool>>(combineOperator(leftBody, rightBody), leftParameter);
        //}

        //private class ReplaceParameterVisitor : ExpressionVisitor
        //{
        //    private readonly ParameterExpression _oldParameter;
        //    private readonly ParameterExpression _newParameter;

        //    public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        //    {
        //        _oldParameter = oldParameter;
        //        _newParameter = newParameter;
        //    }

        //    protected override Expression VisitParameter(ParameterExpression node)
        //    {
        //        return ReferenceEquals(node, _oldParameter) ? _newParameter : base.VisitParameter(node);
        //    }
        //}
        //   public static Func<T, bool> AndAlso<T>(this Func<T, bool> left, Func<T, bool> right)
        //=> a => left(a) && right(a);

        //   public static Func<T, bool> Or<T>(this Func<T, bool> left, Func<T, bool> right)
        //       => a => left(a) || right(a);
    }


}
