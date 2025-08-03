# Feature Development Template

## 🎯 Feature Overview
**Feature Name:** [Enter feature name]
**Priority:** [High/Medium/Low]
**Estimated Complexity:** [Simple/Medium/Complex]

---

## 📋 Phase 1: Requirements Analysis

### Business Context
- **User Story:** [Describe the user workflow and business need]
- **Business Rules:** [List specific business rules and validation requirements]
- **User Roles:** [Who will use this feature and with what permissions]
- **Integration Points:** [How does this connect to existing features]

### Technical Requirements
- **Data Model Changes:** [What entities/tables need modification]
- **API Requirements:** [What endpoints are needed]
- **UI/UX Requirements:** [What the interface should look like]
- **Performance Requirements:** [Any specific performance constraints]
- **Security Considerations:** [Authentication, authorization, data protection]

### Questions to Clarify Before Implementation
- [ ] What is the exact user workflow?
- [ ] What are the validation rules for each field?
- [ ] How should errors be handled and displayed?
- [ ] What are the responsive design requirements?
- [ ] Are there any accessibility requirements?
- [ ] How does this integrate with existing features?
- [ ] What are the edge cases to consider?

---

## 🏗️ Phase 2: Technical Architecture Planning

### Database Changes
- [ ] **Entity Modifications:**
  - [ ] New entities needed
  - [ ] Existing entity updates
  - [ ] Relationship changes
  - [ ] Migration strategy

- [ ] **Data Considerations:**
  - [ ] Data migration requirements
  - [ ] Default values
  - [ ] Constraint requirements
  - [ ] Index requirements

### Backend Implementation
- [ ] **API Endpoints:**
  - [ ] Controller methods needed
  - [ ] HTTP methods (GET, POST, PUT, DELETE)
  - [ ] Route patterns
  - [ ] Authentication requirements

- [ ] **Service Layer:**
  - [ ] Service interfaces
  - [ ] Business logic implementation
  - [ ] Error handling strategy
  - [ ] Validation logic

- [ ] **Repository Layer:**
  - [ ] Repository methods needed
  - [ ] Query optimization
  - [ ] Include statements for related data
  - [ ] Transaction requirements

- [ ] **DTOs/Models:**
  - [ ] Request DTOs
  - [ ] Response DTOs
  - [ ] Validation attributes
  - [ ] Mapping logic

### Frontend Implementation
- [ ] **Component Structure:**
  - [ ] Component hierarchy
  - [ ] Shared components needed
  - [ ] Routing requirements
  - [ ] Lazy loading considerations

- [ ] **Form Handling:**
  - [ ] Form validation rules
  - [ ] Conditional validation
  - [ ] Error state handling
  - [ ] Loading states

- [ ] **API Integration:**
  - [ ] Service methods needed
  - [ ] Error handling
  - [ ] Loading indicators
  - [ ] Success feedback

- [ ] **UI/UX Elements:**
  - [ ] Material Design components
  - [ ] Responsive layout
  - [ ] Accessibility features
  - [ ] Animation requirements

---

## 📁 Phase 3: Implementation Checklist

### Pre-Implementation Review
- [ ] **Study Existing Patterns:**
  - [ ] Review similar components
  - [ ] Check existing service patterns
  - [ ] Examine DTO structures
  - [ ] Understand routing patterns

- [ ] **Project Standards Compliance:**
  - [ ] Use `@main` and `@core` path aliases
  - [ ] Follow component organization rules
  - [ ] Adhere to DateTime handling rules
  - [ ] Use repository pattern consistently

### Database Implementation
- [ ] **Entity Updates:**
  - [ ] Add/update entity properties
  - [ ] Configure relationships properly
  - [ ] Add validation attributes
  - [ ] Use correct data types (timestamp without timezone)

- [ ] **DbContext Configuration:**
  - [ ] Update OnModelCreating
  - [ ] Configure relationships explicitly
  - [ ] Set up proper foreign keys
  - [ ] Avoid shadow properties

- [ ] **Migration:**
  - [ ] Create migration
  - [ ] Review migration for data loss
  - [ ] Test migration on development
  - [ ] Apply migration

### Backend Implementation
- [ ] **API Controller:**
  - [ ] Create controller with proper route
  - [ ] Implement all required endpoints
  - [ ] Add proper HTTP attributes
  - [ ] Include error handling

- [ ] **Service Layer:**
  - [ ] Create service interface
  - [ ] Implement business logic
  - [ ] Handle DateTime conversions (local time)
  - [ ] Add proper error handling

- [ ] **Repository Layer:**
  - [ ] Add repository methods
  - [ ] Use Include() for GET operations
  - [ ] Avoid Include() for INSERT/UPDATE
  - [ ] Optimize queries

- [ ] **DTOs:**
  - [ ] Create request/response DTOs
  - [ ] Add validation attributes
  - [ ] Handle optional fields properly
  - [ ] Use proper data types

### Frontend Implementation
- [ ] **Component Creation:**
  - [ ] Create .ts, .html, .scss files separately
  - [ ] Follow component organization rules
  - [ ] Use proper imports and exports
  - [ ] Add to index files

- [ ] **Form Implementation:**
  - [ ] Create reactive forms
  - [ ] Add proper validation
  - [ ] Handle conditional validation
  - [ ] Implement error display

- [ ] **API Integration:**
  - [ ] Create service methods
  - [ ] Add to api-endpoints.ts
  - [ ] Handle loading states
  - [ ] Implement error handling

- [ ] **Routing:**
  - [ ] Add routes to app.routes.ts
  - [ ] Follow existing route patterns
  - [ ] Add proper guards
  - [ ] Test navigation

### Integration Points
- [ ] **Update Existing Components:**
  - [ ] Add navigation links
  - [ ] Update action buttons
  - [ ] Modify existing forms if needed
  - [ ] Test integration

- [ ] **Service Registration:**
  - [ ] Register new services in DI
  - [ ] Update Program.cs
  - [ ] Test dependency injection

---

## 🧪 Phase 4: Testing Strategy

### Backend Testing
- [ ] **Unit Tests:**
  - [ ] Service layer tests
  - [ ] Repository tests
  - [ ] Controller tests
  - [ ] DTO validation tests

- [ ] **Integration Tests:**
  - [ ] API endpoint tests
  - [ ] Database integration tests
  - [ ] Authentication tests

### Frontend Testing
- [ ] **Component Tests:**
  - [ ] Form validation tests
  - [ ] API integration tests
  - [ ] Error handling tests
  - [ ] Navigation tests

### Manual Testing
- [ ] **User Workflow:**
  - [ ] Test complete user journey
  - [ ] Verify all business rules
  - [ ] Test error scenarios
  - [ ] Check responsive design

- [ ] **Integration Testing:**
  - [ ] Test with existing features
  - [ ] Verify data consistency
  - [ ] Check performance impact
  - [ ] Test edge cases

---

## 🔧 Phase 5: Quality Assurance

### Code Quality
- [ ] **Linting:**
  - [ ] No TypeScript errors
  - [ ] No C# compilation errors
  - [ ] Follow coding standards
  - [ ] Proper naming conventions

- [ ] **Build Verification:**
  - [ ] Frontend builds successfully
  - [ ] Backend builds successfully
  - [ ] No migration issues
  - [ ] All dependencies resolved

### Performance
- [ ] **Database:**
  - [ ] Query optimization
  - [ ] Proper indexing
  - [ ] No N+1 queries
  - [ ] Efficient data loading

- [ ] **Frontend:**
  - [ ] Lazy loading implemented
  - [ ] Bundle size optimized
  - [ ] API calls optimized
  - [ ] Responsive performance

### Security
- [ ] **Authentication:**
  - [ ] Proper authorization checks
  - [ ] JWT token validation
  - [ ] Role-based access control
  - [ ] Input validation

- [ ] **Data Protection:**
  - [ ] SQL injection prevention
  - [ ] XSS protection
  - [ ] CSRF protection
  - [ ] Sensitive data handling

---

## 📝 Phase 6: Documentation

### Technical Documentation
- [ ] **API Documentation:**
  - [ ] Endpoint descriptions
  - [ ] Request/response examples
  - [ ] Error codes
  - [ ] Authentication requirements

- [ ] **Code Documentation:**
  - [ ] Method comments
  - [ ] Complex logic explanations
  - [ ] Business rule documentation
  - [ ] Architecture decisions

### User Documentation
- [ ] **Feature Description:**
  - [ ] What the feature does
  - [ ] How to use it
  - [ ] Common scenarios
  - [ ] Troubleshooting guide

---

## 🚀 Phase 7: Deployment Checklist

### Pre-Deployment
- [ ] **Code Review:**
  - [ ] All changes reviewed
  - [ ] No breaking changes
  - [ ] Backward compatibility
  - [ ] Performance impact assessed

- [ ] **Testing:**
  - [ ] All tests passing
  - [ ] Manual testing completed
  - [ ] Integration testing done
  - [ ] Performance testing

### Deployment
- [ ] **Database:**
  - [ ] Migration applied
  - [ ] Data verified
  - [ ] Backup created
  - [ ] Rollback plan ready

- [ ] **Application:**
  - [ ] Code deployed
  - [ ] Configuration updated
  - [ ] Services restarted
  - [ ] Health checks passing

### Post-Deployment
- [ ] **Verification:**
  - [ ] Feature working in production
  - [ ] No errors in logs
  - [ ] Performance monitoring
  - [ ] User feedback collection

---

## 🎯 Project-Specific Standards

### HalloDoc Project Rules
- [ ] **Always use repository pattern for database access**
- [ ] **Use [FromForm] for file upload endpoints**
- [ ] **All DateTime fields use [Column(TypeName = "timestamp without time zone")]**
- [ ] **Use DateTime.Now for timestamp without timezone columns**
- [ ] **All navigation properties must be explicitly mapped in OnModelCreating**
- [ ] **Add ICollection<T> navigation properties for collection relationships**
- [ ] **Use FormData in frontend for file uploads**
- [ ] **Use Entity Framework includes for GET operations**

### Frontend Standards
- [ ] **Use @main and @core path aliases**
- [ ] **Create separate .ts, .html, .scss files**
- [ ] **Organize components in feature-based folders**
- [ ] **Use Angular standalone components**
- [ ] **Follow Material Design patterns**
- [ ] **Implement proper form validation**
- [ ] **Handle loading and error states**
- [ ] **Use and create enums and utility/helper functions wherever possible**

### Backend Standards
- [ ] **Follow existing controller patterns**
- [ ] **Use proper DTOs for data transfer**
- [ ] **Implement proper error handling**
- [ ] **Use dependency injection**
- [ ] **Follow naming conventions**
- [ ] **Add proper validation attributes**
- [ ] **Use and create enums and utility/helper functions wherever possible**

---

## 📋 Template Usage Instructions

1. **Copy this template for each new feature**
2. **Fill in the specific details for your feature**
3. **Check off items as you complete them**
4. **Update the template based on lessons learned**
5. **Share completed template with team**

---

## 🎯 Success Criteria

A feature is considered successfully implemented when:
- [ ] All business requirements are met
- [ ] All technical requirements are satisfied
- [ ] Code follows project standards
- [ ] All tests are passing
- [ ] Performance is acceptable
- [ ] Security requirements are met
- [ ] Documentation is complete
- [ ] User acceptance criteria are met 