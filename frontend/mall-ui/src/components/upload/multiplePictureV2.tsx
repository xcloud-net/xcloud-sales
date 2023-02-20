import { Button, Card, Image, message, Tooltip as XTooltip } from 'antd';
import { forwardRef, useEffect, useRef, useState } from 'react';
import { ReactSortable } from 'react-sortablejs';

import u from '@/utils';
import utils from '@/utils/manage';
import { MallStorageMetaDto } from '@/utils/models';
import { Close } from '@mui/icons-material';
import { Alert, Box, Grid, IconButton, Tooltip } from '@mui/material';

const CustomComponent = forwardRef<HTMLDivElement, any>((props, ref) => {
  return (
    <Grid ref={ref} container spacing={1} sx={{}}>
      {props.children}
    </Grid>
  );
});

export default (props: {
  data: MallStorageMetaDto[];
  ok: any;
  loading: boolean;
  title?: string;
  count?: number;
  maxSize?: number;
}) => {
  const { data, ok, count, maxSize, loading, title } = props;

  const [storage, _storage] = useState<MallStorageMetaDto[]>([]);
  const [loadingSave, _loadingSave] = useState(false);
  const [loadingUpload, _loadingUpload] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  const pictureCountLimit = count || 20;
  const pictureCountRemain = pictureCountLimit - storage.length;
  const pictureMaxSize = maxSize || 1024 * 1024 * 1;

  const uploadFile = async (file: File) => {
    try {
      file = await utils.convertFileV2(file);
      file = await utils.compressImage(file);
      if (file.size > pictureMaxSize) {
        message.error('图片尺寸过大');
        return;
      }

      var response = await utils.uploadFileAndSavePicture(file);

      if (response && response != undefined) {
        const uploadedStorageMeta: MallStorageMetaDto = {
          ...(response.StorageMeta || {}),
          PictureId: response.Id,
        };
        _storage((x) => [...x, uploadedStorageMeta]);
      } else {
        message.error('上传失败');
      }
    } catch (e) {
      console.log(e);
      message.error('上传遇到错误');
    } finally {
      //
    }
  };

  const uploadMultipleFile = async (files: File[]) => {
    _loadingUpload(true);
    try {
      for (var i = 0; i < files.length; ++i) {
        var file = files[i];
        if (!utils.isImage(file.name)) {
          message.error(
            `只支持 ${utils.allowedImageExtensions().join('、')} 格式的图片`,
          );
          continue;
        }

        await uploadFile(file);
      }
    } finally {
      _loadingUpload(false);
    }
  };

  useEffect(() => {
    _storage(data || []);
  }, [data]);

  const renderPictureBox = (picture: MallStorageMetaDto) => {
    var url = u.resolveUrlv2(picture, {
      height: 200,
      width: 200,
    });

    return (
      <>
        <Box
          sx={{
            display: 'inline-block',
            position: 'relative',
            p: 2,
          }}
        >
          <Box
            sx={{
              display: 'inline-block',
              position: 'absolute',
              zIndex: 999,
              top: 0,
              right: 0,
            }}
          >
            <Tooltip title="删除">
              <IconButton
                sx={{
                  backgroundColor: (theme) => theme.palette.error.light,
                  color: 'white',
                  '&:hover': {
                    backgroundColor: (theme) => theme.palette.error.main,
                  },
                }}
                onClick={() => {
                  _storage(u.filter(storage, (x) => x.Id != picture.Id));
                }}
                size="small"
              >
                <Close fontSize="small" />
              </IconButton>
            </Tooltip>
          </Box>
          <Image src={url as string} width={100} height={100} alt="" />
        </Box>
      </>
    );
  };

  return (
    <>
      <div style={{ display: 'none' }}>
        <input
          ref={inputRef}
          type={'file'}
          accept="image/*"
          multiple={true}
          onChange={(e) => {
            if (e.target.files && e.target.files.length > 0) {
              if (e.target.files.length > pictureCountRemain) {
                message.error(
                  `还可以上传${pictureCountRemain}张图片，你选择的太多了`,
                );
                return;
              }
              var filelist: File[] = [];
              for (var i = 0; i < e.target.files.length; ++i) {
                filelist.push(e.target.files[i]);
              }
              uploadMultipleFile(filelist);
            }
          }}
        />
      </div>
      <Card
        title={title || '--'}
        loading={loading}
        extra={
          <Button.Group>
            <XTooltip title="如果你修改了排序，但是不想保存">
              <Button
                type="dashed"
                onClick={() => {
                  _storage(data || []);
                }}
              >
                放弃顺序修改
              </Button>
            </XTooltip>
            <Button
              type="primary"
              loading={loadingSave}
              disabled={loadingUpload}
              onClick={() => {
                ok && ok(storage);
              }}
            >
              {loadingUpload ? `上传中，请稍后` : `保存`}
            </Button>
          </Button.Group>
        }
      >
        <Box sx={{ mb: 2 }}>
          <Alert security="info">
            <p>{`还可以上传${pictureCountRemain}张图片`}</p>
            <p>拖动图片可以排序，第一张图片将作为商品主图</p>
          </Alert>
        </Box>
        <ReactSortable
          tag={CustomComponent}
          list={storage.map((x) => ({ ...x, id: x.Id || 0 }))}
          animation={150}
          group="cards"
          setList={(x) => {
            _storage(x);
          }}
        >
          {u.map(storage, (x) => (
            <Grid item xs={2} key={x.Id}>
              <Box sx={{}}>{renderPictureBox(x)}</Box>
            </Grid>
          ))}
        </ReactSortable>

        {pictureCountRemain > 0 && (
          <Button
            loading={loading}
            type="dashed"
            onClick={() => {
              inputRef.current?.click();
            }}
          >
            选择图片
          </Button>
        )}
      </Card>
    </>
  );
};
