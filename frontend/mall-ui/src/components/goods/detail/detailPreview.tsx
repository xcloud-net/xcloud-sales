import XDetailDialog from '@/components/detailDialog';
import XDetail from './index';

const index = function (props: any) {
  const { detailId, onClose } = props;

  const active = detailId > 0;

  return (
    <>
      <XDetailDialog open={active} onClose={onClose}>
        <XDetail goodsId={detailId} />
      </XDetailDialog>
    </>
  );
};

export default index;
