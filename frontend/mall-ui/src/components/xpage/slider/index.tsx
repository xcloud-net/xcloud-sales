import XEmptyImage from '@/assets/empty.svg';
import u from '@/utils';
import { IPageSliderItem, PageSlider } from '@/utils/models';
import { Box, Alert } from '@mui/material';
import { Swiper, Image } from 'antd-mobile';

interface SliderProps {
  data: IPageSliderItem;
}

export default function IndexPage(props: SliderProps) {
  const { data } = props;

  const renderSliderItem = (item: PageSlider, index: any) => {
    var img = u.resolveUrlv2(item.img);
    return (
      <Box
        sx={{
          overflow: 'hidden',
          width: '100%',
          height: {
            xs: 300,
            sm: 300,
            md: 400,
            lg: 450,
          },
        }}
      >
        <Image
          src={img || XEmptyImage}
          alt=""
          style={{
            height: '100%',
            width: '100%',
          }}
        />
      </Box>
    );
  };

  if (!data || data.type != 'slider') {
    return null;
  }

  const { sliders } = data;

  if (u.isEmpty(sliders)) {
    return null;
  }

  return (
    <>
      <Box sx={{}}>
        {u.isEmpty(sliders) && <Alert>轮播图</Alert>}
        {u.isEmpty(sliders) || (
          <Swiper slideSize={100}>
            {u.map(sliders, (item: PageSlider, index) => (
              <Swiper.Item key={index}>
                {renderSliderItem(item, index)}
              </Swiper.Item>
            ))}
          </Swiper>
        )}
      </Box>
    </>
  );
}
