import u from '@/utils';
import { Box } from '@mui/material';
import XContent from './content';
import XGoodsCollection from './goodsCollection';
import XPicture from './picture';
import XSlider from './slider';
import XVideo from './video';

export default function IndexPage(props: { data?: any[] }) {
  const { data } = props;

  if (u.isEmpty(data)) {
    return null;
  }

  return (
    <>
      <Box sx={{}}>
        {u.map(data || [], (item, index) => (
          <Box
            key={index}
            sx={{
              ...(item.sx || {}),
            }}
          >
            <XVideo data={item} />
            <XSlider data={item} />
            <XContent data={item} />
            <XGoodsCollection data={item} />
            <XPicture data={item} />
          </Box>
        ))}
      </Box>
    </>
  );
}
