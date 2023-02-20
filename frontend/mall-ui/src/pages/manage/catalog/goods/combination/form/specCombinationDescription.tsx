import { GoodsCombinationDto } from '@/utils/models';

export default ({ model }: { model: GoodsCombinationDto }) => {
  const specs = model.ParsedSpecificationsJson || [];
  const errors = model.SpecCombinationErrors || [];

  return (
    <>
      <div style={{}}>
        {specs.map((x, i) => (
          <div style={{ marginBottom: 5 }} key={i}>
            {`${x.Spec?.Name || '--'}:${x.SpecValue?.Name || '--'}`}
          </div>
        ))}
        {errors.map((x, i) => (
          <div
            style={{ marginBottom: 5, color: 'red', fontWeight: 'bold' }}
            key={i}
          >
            {x || '--'}
          </div>
        ))}
      </div>
    </>
  );
};
