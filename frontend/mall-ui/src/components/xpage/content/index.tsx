import XMD from '@/components/markdown';
import { Box } from '@mui/material';
import { IPageContentItem } from '@/utils/models';

interface ContentProps {
  data: IPageContentItem;
}

export default function IndexPage(props: ContentProps) {
  const { data } = props;

  if (!data || data.type != 'content') {
    return null;
  }

  const { content } = data;

  return (
    <>
      <Box>
        <XMD md={content} />
      </Box>
    </>
  );
}
