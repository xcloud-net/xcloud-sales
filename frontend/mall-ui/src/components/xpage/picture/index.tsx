import u from '@/utils';
import { IPagePictureItem } from '@/utils/models';
import { Alert, Box } from '@mui/material';
import { Image } from 'antd-mobile';

interface PictureProps {
  data: IPagePictureItem;
}

export default function IndexPage(props: PictureProps) {
  const {
    data,
    data: { type },
  } = props;

  if (type != 'picture') {
    return null;
  }

  var pictureUrl = u.resolveUrlv2(data.picture, { width: 800 });

  return (
    <Box
      sx={{}}
      onClick={() => {
        data.link && u.go(data.link);
      }}
    >
      {u.isEmpty(pictureUrl) || (
        <Image alt="" src={pictureUrl as string} fit="contain" lazy />
      )}
      {u.isEmpty(pictureUrl) && <Alert>图片模块</Alert>}
    </Box>
  );
}
