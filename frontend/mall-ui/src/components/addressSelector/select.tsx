import u from '@/utils';

import { Box, OutlinedInput, InputLabel, MenuItem, FormControl, Select, Chip } from '@mui/material';

export default function MultipleSelectChip(props: any) {
  const { data, selected, label, onChange } = props;

  const renderValue = (selectedValue: string) => {
    var item = u.find(data, (x) => x.Id == selectedValue);
    if (!item) {
      return '';
    }

    return (
      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
        <Chip key={item.Id} label={item.Name} />
      </Box>
    );
  };

  return (
    <div>
      <FormControl sx={{ marginBottom: 1, marginTop: 1, minWidth: 100 }}>
        <InputLabel id="demo-multiple-chip-label">{label}</InputLabel>
        <Select
          labelId="demo-multiple-chip-label"
          value={selected}
          onChange={(e) => {
            console.log(e.target.value);
            onChange && onChange(e.target.value);
          }}
          input={<OutlinedInput label="Chip" />}
          renderValue={(selected) => {
            return renderValue(selected);
          }}
        >
          {u.isEmpty(data) ||
            u.map(data, (x) => {
              return (
                <MenuItem selected={selected == x.Id} key={x.Id} value={x.Id}>
                  {x.Name}
                </MenuItem>
              );
            })}
        </Select>
      </FormControl>
    </div>
  );
}
