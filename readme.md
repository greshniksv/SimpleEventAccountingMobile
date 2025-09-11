# Build project
dotnet publish -f:net9.0-android -c Release -o ./publish /p:AndroidPackageFormat=apk

# Migration
dotnet ef migrations add AddChangeSet --project SimpleEventAccountingMobile --framework net9.0-windows10.0.19041.0

# TODO

## Client group
- [x] Add clients group
- [x] Use client group in client selection popup
- [x] Use client group in create training page
- [x] Think about create wizard for create training page

## Reports
- [ ] Think about report functionality


## Bug fixing
- [ ] Allow user to send bug report as mail

## Fix
- [ ] Settings page

## License
- [x] Add settings table
- [ ] Create generator of keys
- [ ] Create page to add key and functionality to validate
- [ ] Validate lic key on add client
- [ ] Different lic key for reports

## Language
- [ ] Add english language