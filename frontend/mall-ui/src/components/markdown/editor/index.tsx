import u from '@/utils';
import { message } from 'antd';
import { useEffect, useState } from 'react';
import MdEditor from 'react-markdown-editor-lite';
// import style manually
import utils from '@/utils/manage';
import ReactMarkdown from 'react-markdown';
import 'react-markdown-editor-lite/lib/index.css';
import LinearProgress from '@/components/loading/linear';

export default (props: any) => {
  const { onChange, value, style } = props;
  const [uploading, _uploading] = useState(false);

  const [md, _md] = useState('');

  useEffect(() => {
    _md(value);
  }, [value]);

  return (
    <>
      {uploading && <LinearProgress />}
      <MdEditor
        style={{ height: '500px', ...(style || {}) }}
        value={md}
        renderHTML={(text) => (
          <ReactMarkdown remarkPlugins={[]}>{text}</ReactMarkdown>
        )}
        onChange={({ text, html }) => {
          _md(text);
          onChange && onChange(text);
        }}
        onImageUpload={(file: File) =>
          new Promise(async (resolve, reject) => {
            try {
              _uploading(true);
              file = await utils.convertFileV2(file);
              file = await utils.compressImage(file);
              if (file.size > 1024 * 1024 * 1) {
                message.error('图片大小不能超过1MB');
                reject('图片大小不能超过1MB');
                return;
              }

              var response = await utils.uploadFileAndSavePicture(file);
              if (response) {
                var url = u.resolveUrlv2(response.StorageMeta, {
                  width: 500,
                  //height: 500,
                });
                resolve(url);
              } else {
                message.error('上传失败');
                reject('上传失败');
              }
            } catch (e) {
              reject(e);
            } finally {
              _uploading(false);
            }
          })
        }
      />
    </>
  );
};
