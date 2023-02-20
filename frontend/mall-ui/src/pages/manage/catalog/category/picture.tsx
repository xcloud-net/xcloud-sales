import XSinglePictureUploader from '@/components/upload/singlePicture';
import u from '@/utils';
import { message } from 'antd';
import { useState } from 'react';

export default (props: any) => {
  const { data, ok } = props;

  const [loading, _loading] = useState(false);

  const setPicture = (pictureId: any) => {
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/category/set-picture', {
        Id: data.Id,
        PictureId: pictureId || 0,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          message.success('修改成功');
          ok && ok();
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  return (
    <>
      <XSinglePictureUploader
        loadingSave={loading}
        data={data.Picture}
        ok={(res: any) => {
          setPicture(res.Id);
        }}
        remove={() => {
          if (confirm('确定删除图片吗？')) {
            setPicture(0);
          }
        }}
      />
    </>
  );
};
