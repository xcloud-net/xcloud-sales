import u from '@/utils';
import http from '@/utils/http';
import { AxiosResponse } from 'axios';

interface IOnLoginOption {
  onSuccess: Function;
  onError: Function;
}

export default {
  onLogin: (option: IOnLoginOption) => {
    const { onSuccess, onError } = option;
    http.apiRequest
      .post('/mall/user/profile')
      .then((res) => {
        if (res.data.Error) {
          onError && onError(res.data.Error);
        } else {
          onSuccess && onSuccess(res.data.Data);
        }
      })
      .catch((e) => {
        onError && onError(e);
      });
  },

  queryShoppingcartCount: () => {
    return new Promise<any>((resolve, reject) => {
      http.apiRequest
        .post('/mall/shoppingcart/count', {})
        .then((res) => {
          var count = res.data.Data || 0;
          resolve(count);
        })
        .catch((err) => {
          console.log(
            'read user unread notifications count',
            err?.response?.data,
          );
          reject(err);
        });
    });
  },
  queryNotificationCount: () => {
    return new Promise<any>((resolve, reject) => {
      http.platformRequest
        .post('/user/notification/unread-count', {})
        .then((res) => {
          var count = res.data.Data || 0;
          resolve(count);
        })
        .catch((err) => {
          console.log(
            'read user unread notifications count',
            err?.response?.data,
          );
          reject(err);
        });
    });
  },
  addFavorite: (id: any) => {
    return new Promise<AxiosResponse>((resolve, reject) => {
      http.apiRequest
        .post('/mall/favorites/like', {
          Id: id,
        })
        .then((res) => {
          resolve(res);
        })
        .catch((e) => reject(e));
    });
  },
  removeFavorite: (id: any) => {
    return new Promise<AxiosResponse>((resolve, reject) => {
      http.apiRequest
        .post('/mall/favorites/unlike', {
          Id: id,
        })
        .then((res) => {
          resolve(res);
        })
        .catch((e) => reject(e));
    });
  },
};
