
```csharp

        [NonAction, Obsolete("用中间件或者exception handler拦截异常")]
        async Task<ActionResult> RunActionAsync(Func<Task<ActionResult>> GetActionFunc)
        {
            try
            {
                var data = await GetActionFunc.Invoke();

                return data;
            }
#if !xx
            catch (Exception e)
            {
                var err_handler = this.HttpContext.RequestServices.GetRequiredService<MvcExceptionHandler>();
                var response = await err_handler.ConvertToErrorInfo(e);
                return new JsonResult(response);
            }
#else
            catch (MsgException e)
            {
                return GetJsonRes(e.Message);
            }
            catch (NoParamException e)
            {
                return GetJson(new ResponseEntity()
                {
                    Success = false,
                    ErrorMsg = "参数错误",
                    ErrorCode = "-100",
                    Data = e.ResponseTemplate
                });
            }
            catch (NoTenantException)
            {
                return GetJson(new ResponseEntity()
                {
                    Success = false,
                    ErrorMsg = "请选择组织",
                    ErrorCode = "-111"
                });
            }
            catch (NoLoginException)
            {
                return GetJson(new ResponseEntity()
                {
                    Success = false,
                    ErrorMsg = "没有登录",
                    ErrorCode = "-401"
                });
            }
            catch (NoPermissionInTenantException)
            {
                return GetJson(new ResponseEntity()
                {
                    Success = false,
                    ErrorMsg = "没有权限",
                    ErrorCode = "-403"
                });
            }
            catch (NotImplementedException)
            {
                return GetJson(new ResponseEntity()
                {
                    Success = false,
                    ErrorMsg = "功能没有实现",
                    ErrorCode = "-1111"
                });
            }
            catch (Exception e)
            {
                e.AddErrorLog();
                return GetJsonRes("error");
            }
#endif
        }
```