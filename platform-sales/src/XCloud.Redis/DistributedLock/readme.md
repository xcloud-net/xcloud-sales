# Redis分布式锁

https://github.com/samcook/RedLock.net

- 必须考虑到【锁的续租】，redis key是会过期的，如果拿到锁后未能很快处理完任务，那么要对锁进行续期。
- 否则别人就会拿到锁，抢占你的资源