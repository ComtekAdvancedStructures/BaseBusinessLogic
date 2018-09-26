using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using Comtek.Helpers;

namespace Comtek
{
    public abstract class BaseBusinessLogic<T> where T : DbContext
    {
        private readonly string successMessage;
        private readonly string failMessage;
        public T Context { get; }

        protected BaseBusinessLogic(T context, string appName)
        {
            Context = context;
            successMessage = $"Changes saved to {appName} database";
            failMessage = $"There was an error saving the changes to the {appName} database.";
        }  
         
        public DataOperationResult Save()
        {
            var result = new DataOperationResult();
            try
            {
                Context.SaveChanges();
                result.Success = true;
                result.Messages.Add(successMessage);
            }
            catch (DbEntityValidationException e)
            {
                result.Success = false;
                result.Messages.Add(failMessage);
                foreach (var eve in e.EntityValidationErrors)
                {
                    result.Messages.Add(
                        $"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors:");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        result.Messages.Add($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
                    }
                }                
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Messages.Add(failMessage);
                result.Messages.Add(ErrorHelper.GetExceptionMessage(ex));
            }
            return result;
        }

        public async Task<DataOperationResult> SaveAsync()
        {
            var result = new DataOperationResult();
            try
            {
                await Context.SaveChangesAsync();
                result.Success = true;
                result.Messages.Add(successMessage);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Messages.Add(failMessage);
                result.Messages.Add(ErrorHelper.GetExceptionMessage(ex));
            }
            return result;
        }
    }
}
