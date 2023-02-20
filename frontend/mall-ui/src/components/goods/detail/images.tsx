import XEmptyImage from '@/assets/empty.svg';
import u from '@/utils';
import { GoodsDto } from '@/utils/models';
import { Box } from '@mui/material';
import { Image, ImageViewer, Swiper } from 'antd-mobile';
import { useState } from 'react';

export default (props: { model: GoodsDto; combinationId?: number }) => {
  const { model, combinationId } = props;

  const pictureList = u.sortBy(
    model.XPictures || [],
    (x) => (x.CombinationId == combinationId ? 0 : 1),
    (x) => x.DisplayOrder,
  );

  const imageUrlList = pictureList.map((x) => ({
    origin: u.resolveUrlv2(x, { width: 900 }),
    small: u.resolveUrlv2(x, { width: 500 }),
  }));
  const [previewIndex, _previewIndex] = useState(0);
  const [previewShow, _previewShow] = useState(false);

  return (
    <>
      <ImageViewer.Multi
        visible={previewShow}
        //read only once,so dosent work
        defaultIndex={previewIndex}
        images={imageUrlList.map((x) => x.origin)}
        onClose={() => {
          _previewShow(false);
        }}
      />
      <Box
        sx={{
          minHeight: '200px',
          mb: 2,
        }}
      >
        {u.isEmpty(imageUrlList) && (
          <img src={XEmptyImage} style={{ width: '100%' }} alt="" />
        )}
        {u.isEmpty(imageUrlList) || (
          <Swiper slideSize={100}>
            {imageUrlList.map((x, index) => (
              <Swiper.Item key={index}>
                <Box
                  sx={{
                    width: '100%',
                    height: {
                      xs: 350,
                      sm: 350,
                      md: 450,
                      lg: 500,
                    },
                  }}
                  onClick={() => {
                    _previewIndex(index);
                    _previewShow(true);
                  }}
                >
                  <Image
                    alt=""
                    lazy={false}
                    src={x.small}
                    fit="cover"
                    style={{
                      height: '100%',
                      width: '100%',
                    }}
                  />
                </Box>
              </Swiper.Item>
            ))}
          </Swiper>
        )}
      </Box>
    </>
  );
};
