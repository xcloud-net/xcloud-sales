import emptyImage from '@/assets/empty.svg';
import XImage from '@/components/image';
import u from '@/utils';
import { GoodsDto } from '@/utils/models';
import { Box } from '@mui/material';

const index = function (props: { model: GoodsDto; lazy?: boolean }) {
  const { model, lazy } = props;

  if (!model || !model.Id) {
    return null;
  }

  const resolvePictureUrl = () => {
    var firstPicture = u.first(model.XPictures || []);
    var url = null;
    if (firstPicture) {
      url = u.resolveUrlv2(firstPicture, {
        width: 250,
        height: 250,
      });
    }
    url = url || emptyImage;
    return url;
  };

  return (
    <>
      <Box sx={{}}>
        <XImage
          style={{
            width: '100%',
          }}
          src={resolvePictureUrl()}
          alt="Random unsplash image"
          fit="contain"
          lazy={lazy}
        />
      </Box>
    </>
  );
};

export default index;
