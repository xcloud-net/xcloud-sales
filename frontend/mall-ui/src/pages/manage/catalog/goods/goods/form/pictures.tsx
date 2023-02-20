import { message } from 'antd';
import { useEffect, useState } from 'react';

import XMultiplePictureUploader from '@/components/upload/multiplePictureV2';
import u from '@/utils';
import http from '@/utils/http';
import {
  GoodsCombinationDto,
  GoodsDto,
  MallStorageMetaDto
} from '@/utils/models';

export default (props: { data: GoodsDto; ok: any }) => {
  const { data, ok } = props;

  const [loadingSave, _loadingSave] = useState(false);
  const [loading, _loading] = useState(false);
  const [combination, _combination] = useState<GoodsCombinationDto[]>([]);

  useEffect(() => {
    queryList();
  }, [data]);

  const queryList = () => {
    if (!data || !data.Id) {
      return;
    }
    _loading(true);

    http.apiRequest
      .post('/mall-admin/combination/list-by-goodsid', {
        Id: data.Id,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _combination(res.data.Data || []);
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const saveCombinationsImages = (
    combinationId: number,
    storage: MallStorageMetaDto[],
  ) => {
    _loadingSave(true);

    http.apiRequest
      .post('/mall-admin/goods/save-combination-images-v1', {
        Id: data.Id,
        CombinationId: combinationId,
        //PictureIds: u.map(storage, (x) => x.Id),
        PictureIdArray: (storage || []).map((x, index) => ({
          Id: x.PictureId,
          Index: index,
        })),
      })
      .then((res) => {
        u.handleResponse(res, () => {
          message.success('保存成功');
          ok && ok();
        });
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  return (
    <>
      <div style={{ marginBottom: 10 }}>
        <XMultiplePictureUploader
          title="商品图片"
          data={(data.XPictures || []).filter(
            (x) => (x.CombinationId || 0) <= 0,
          )}
          ok={(x: MallStorageMetaDto[]) => {
            saveCombinationsImages(0, x);
          }}
          loading={loadingSave}
        />
      </div>
      {loading && <div>loading...</div>}
      {combination.map((x, index) => (
        <div key={index} style={{ marginBottom: 10 }}>
          <XMultiplePictureUploader
            title={x.Name}
            data={(data.XPictures || []).filter((d) => d.CombinationId == x.Id)}
            ok={(imgs: MallStorageMetaDto[]) => {
              saveCombinationsImages(x.Id || 0, imgs);
            }}
            loading={loadingSave}
          />
        </div>
      ))}
    </>
  );
};
