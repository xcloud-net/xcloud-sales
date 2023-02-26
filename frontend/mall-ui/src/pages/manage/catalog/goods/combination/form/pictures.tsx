import { Button, message, Modal, Space } from 'antd';
import React, { useState } from 'react';

import XMultiplePictureUploader from '@/components/upload/multiplePictureV2';
import u from '@/utils';
import http from '@/utils/http';
import { GoodsCombinationDto, MallStorageMetaDto } from '@/utils/models';
import ImgPreview from '@/components/image/PreviewGroup';

export default (props: { data: GoodsCombinationDto; ok: any }) => {
  const { data, ok } = props;

  const [open, _open] = useState(false);
  const [loadingSave, _loadingSave] = useState(false);
  const [loading, _loading] = useState(false);

  const saveCombinationsImages = (
    combinationId: number,
    storage: MallStorageMetaDto[],
  ) => {
    _loadingSave(true);

    http.apiRequest
      .post('/mall-admin/goods/save-combination-images-v1', {
        Id: data.GoodsId,
        CombinationId: data.Id,
        //PictureIds: u.map(storage, (x) => x.Id),
        PictureIdArray: (storage || []).map((x, index) => ({
          Id: x.PictureId,
          Index: index,
        })),
      })
      .then((res) => {
        u.handleResponse(res, () => {
          message.success('保存成功');
          _open(false);
          ok && ok();
        });
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  const pics = data.XPictures || (data.Goods?.XPictures || []).filter(x => x.CombinationId == data.Id) || [];

  return (
    <>
      <Space direction={'horizontal'}>
        <div>
          <ImgPreview
            data={pics}
          />
        </div>
        <Button onClick={() => {
          _open(true);
        }}>修改</Button>
      </Space>
      <Modal open={open} footer={false} title={'规格图片'} width={'90%'} onCancel={() => {
        _open(false);
      }} okText={null}>
        {loading && <div>loading...</div>}
        <div style={{ marginBottom: 10 }}>
          <XMultiplePictureUploader
            title={data.Name}
            data={pics}
            ok={(imgs: MallStorageMetaDto[]) => {
              saveCombinationsImages(data.Id || 0, imgs);
            }}
            loading={loadingSave}
          />
        </div>
      </Modal>
    </>
  );
};
