import u from '@/utils';
import utils from '@/utils/manage';
import { MallStorageMetaDto } from '@/utils/models';
import { Button, Space, message } from 'antd';
import { useRef, useState } from 'react';

export default (props: {
  data: MallStorageMetaDto;
  ok: any;
  remove: any;
  loadingSave?: boolean;
}) => {
  const { data, ok, remove, loadingSave } = props;

  const [loading, _loading] = useState(false);
  const pictureUrl = u.resolveUrlv2(data, { width: 100 });
  const inputRef = useRef<HTMLInputElement>(null);

  const uploadFile = async (file: File) => {
    _loading(true);
    try {
      if (!utils.isImage(file.name)) {
        message.error(
          `只支持 ${utils.allowedImageExtensions().join('、')} 格式的图片`,
        );
        return;
      }

      file = await utils.convertFileV2(file);
      file = await utils.compressImage(file);
      if (file.size > 1024 * 1024 * 1) {
        message.error('图片大小不能超过1MB');
        return;
      }

      var response = await utils.uploadFileAndSavePicture(file);
      if (response) {
        ok && ok(response);
      } else {
        message.error('上传失败');
      }
    } catch (e) {
      console.log(e);
      message.error('上传遇到错误');
    } finally {
      _loading(false);
    }
  };

  return (
    <>
      <div style={{ display: 'none' }}>
        <input
          ref={inputRef}
          type={'file'}
          accept="image/*"
          multiple={false}
          onChange={(e) => {
            if (e.target.files && e.target.files.length > 0) {
              uploadFile(e.target.files[0]);
            }
          }}
        />
      </div>
      <Space direction="vertical">
        {pictureUrl && (
          <img
            src={pictureUrl}
            alt=""
            width={50}
            onClick={() => {
              remove && remove(data);
            }}
          />
        )}

        <Button
          loading={loading}
          type="dashed"
          onClick={() => {
            inputRef.current?.click();
          }}
        >
          选择图片
        </Button>
      </Space>
    </>
  );
};
